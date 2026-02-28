"""Build consolidated question_bank.json for Unity runtime.

Reads the manifest at docs/question_bank.json, loads all referenced paper
files, and produces a single consolidated JSON at
UnityProject/BrainAcademy/Assets/Resources/question_bank.json.

Usage:
    uv run scripts/build_question_bank.py
"""

import json
import sys
from datetime import datetime, timezone
from pathlib import Path

DOCS_DIR = Path(__file__).resolve().parent.parent / "docs"
MANIFEST_PATH = DOCS_DIR / "question_bank.json"
OUTPUT_PATH = (
    Path(__file__).resolve().parent.parent
    / "UnityProject"
    / "BrainAcademy"
    / "Assets"
    / "Resources"
    / "question_bank.json"
)

# Passage types whose questions use standard MCQ format
MCQ_PASSAGE_TYPES = {"comprehension", "poem"}


def load_manifest() -> dict:
    with open(MANIFEST_PATH, encoding="utf-8") as f:
        manifest = json.load(f)
    version = manifest.get("meta", {}).get("version", "1.0")
    if not version.startswith("2"):
        print(f"ERROR: Expected manifest version 2.x, got {version}", file=sys.stderr)
        sys.exit(1)
    return manifest


def load_paper(paper_entry: dict) -> dict:
    path = DOCS_DIR / paper_entry["file"]
    if not path.exists():
        print(f"WARNING: Paper file not found: {path}", file=sys.stderr)
        return {}
    with open(path, encoding="utf-8") as f:
        return json.load(f)


def process_short_paper(
    paper_data: dict,
    paper_id: str,
    section: str,
    math_questions: list,
    thinking_questions: list,
    seen_ids: set,
):
    """Process a short-question paper (maths, thinking, or curated mixed)."""
    if section == "mixed":
        # Curated bank has both mathematical_reasoning and thinking_skills
        for s in ("mathematical_reasoning", "thinking_skills"):
            questions = paper_data.get(s, [])
            target = (
                math_questions if s == "mathematical_reasoning" else thinking_questions
            )
            for q in questions:
                qid = q.get("id", "")
                if qid in seen_ids:
                    continue
                seen_ids.add(qid)
                out = build_short_question(q, paper_id)
                target.append(out)
    else:
        questions = paper_data.get("questions", [])
        target = (
            math_questions
            if section == "mathematical_reasoning"
            else thinking_questions
        )
        for q in questions:
            qid = q.get("id", "")
            if qid in seen_ids:
                continue
            seen_ids.add(qid)
            out = build_short_question(q, paper_id)
            target.append(out)


def build_short_question(q: dict, paper_id: str) -> dict:
    """Normalize a short question for runtime JSON."""
    return {
        "id": q.get("id", ""),
        "question_type": "short",
        "question": q.get("question", ""),
        "options": q.get("options", []),
        "correct_index": q.get("correct_index", 0),
        "correct_answer": q.get("correct_answer", ""),
        "topic": q.get("topic"),
        "subtopic": q.get("subtopic"),
        "difficulty": q.get("difficulty"),
        "explanation": q.get("explanation") or q.get("reasoning"),
        "source_paper": paper_id,
    }


def process_reading_paper(
    paper_data: dict,
    paper_id: str,
    passages_out: list,
    questions_out: list,
    seen_passage_ids: set,
    seen_question_ids: set,
):
    """Process a reading paper with passage-based questions."""
    for passage in paper_data.get("passages", []):
        pid = passage.get("id", "")
        ptype = passage.get("type", "")

        if pid not in seen_passage_ids:
            seen_passage_ids.add(pid)
            passage_out = {
                "id": pid,
                "title": passage.get("title", ""),
                "type": ptype,
                "source_paper": paper_id,
            }
            # Include text for comprehension and poem
            if "text" in passage:
                passage_out["text"] = passage["text"]
            if "author" in passage:
                passage_out["author"] = passage["author"]
            if "source" in passage:
                passage_out["source"] = passage["source"]
            # For non-MCQ types, include their special data
            if ptype == "cloze":
                passage_out["blanks"] = passage.get("blanks", [])
            elif ptype == "sentence_insertion":
                passage_out["gaps"] = passage.get("gaps", [])
                passage_out["sentences"] = passage.get("sentences", [])
                passage_out["correct_mapping"] = passage.get("correct_mapping", {})
                passage_out["extra_sentence"] = passage.get("extra_sentence")
                passage_out["gap_reasoning"] = passage.get("gap_reasoning", {})
                passage_out["extra_sentence_reasoning"] = passage.get(
                    "extra_sentence_reasoning"
                )
                if "footnote" in passage:
                    passage_out["footnote"] = passage["footnote"]
            elif ptype == "extract_matching":
                passage_out["extracts"] = passage.get("extracts", [])
                passage_out["statements"] = passage.get("statements", [])

            passages_out.append(passage_out)

        # Extract standard MCQ questions from comprehension/poem passages
        if ptype in MCQ_PASSAGE_TYPES:
            for q in passage.get("questions", []):
                qid = q.get("id", "")
                if qid in seen_question_ids:
                    continue
                seen_question_ids.add(qid)
                questions_out.append(
                    {
                        "id": qid,
                        "question_type": "long",
                        "passage_id": pid,
                        "question": q.get("question", ""),
                        "options": q.get("options", []),
                        "correct_index": q.get("correct_index", 0),
                        "correct_answer": q.get("correct_answer", ""),
                        "explanation": q.get("reasoning"),
                        "source_paper": paper_id,
                    }
                )


def deduplicate_curated(
    math_questions: list, thinking_questions: list, seen_ids: set
):
    """Remove curated questions that duplicate paper questions.

    Curated questions (MR/TS prefix) that match a paper question on
    correct_answer + first 50 chars of question text are removed.
    Paper questions take precedence since they have cleaner IDs.
    """
    def fingerprint(q: dict) -> str:
        text = q.get("question", "")[:50]
        answer = q.get("correct_answer", "")
        return f"{text}||{answer}"

    # Build fingerprints of paper questions (non-curated)
    paper_fps = set()
    for q in math_questions:
        if not q["id"].startswith("MR"):
            paper_fps.add(fingerprint(q))
    for q in thinking_questions:
        if not q["id"].startswith("TS"):
            paper_fps.add(fingerprint(q))

    # Remove curated duplicates
    before_math = len(math_questions)
    before_thinking = len(thinking_questions)
    math_questions[:] = [
        q
        for q in math_questions
        if not q["id"].startswith("MR") or fingerprint(q) not in paper_fps
    ]
    thinking_questions[:] = [
        q
        for q in thinking_questions
        if not q["id"].startswith("TS") or fingerprint(q) not in paper_fps
    ]
    removed = (before_math - len(math_questions)) + (
        before_thinking - len(thinking_questions)
    )
    if removed:
        print(f"  Deduplicated {removed} curated questions that matched paper questions")


def build():
    manifest = load_manifest()
    papers = manifest.get("papers", [])

    math_questions: list[dict] = []
    thinking_questions: list[dict] = []
    reading_passages: list[dict] = []
    reading_questions: list[dict] = []
    seen_ids: set[str] = set()
    seen_passage_ids: set[str] = set()
    seen_question_ids: set[str] = set()

    for paper_entry in papers:
        paper_id = paper_entry["id"]
        section = paper_entry["section"]
        question_type = paper_entry["question_type"]

        print(f"Loading {paper_id} ({section}, {question_type})...")
        paper_data = load_paper(paper_entry)
        if not paper_data:
            continue

        if question_type == "short":
            process_short_paper(
                paper_data, paper_id, section, math_questions, thinking_questions,
                seen_ids,
            )
        elif question_type == "long":
            process_reading_paper(
                paper_data, paper_id, reading_passages, reading_questions,
                seen_passage_ids, seen_question_ids,
            )

    # Deduplicate curated vs paper questions
    deduplicate_curated(math_questions, thinking_questions, seen_ids)

    # Build runtime JSON
    runtime = {
        "meta": {
            "version": "2.0",
            "generated": datetime.now(timezone.utc).isoformat(),
            "description": "NSW Opportunity Class Placement Test Question Bank (consolidated)",
            "sections": ["mathematical_reasoning", "thinking_skills", "reading"],
        },
        "mathematical_reasoning": math_questions,
        "thinking_skills": thinking_questions,
        "reading": {
            "passages": reading_passages,
            "questions": reading_questions,
        },
    }

    # Write output
    OUTPUT_PATH.parent.mkdir(parents=True, exist_ok=True)
    with open(OUTPUT_PATH, "w", encoding="utf-8") as f:
        json.dump(runtime, f, indent=2, ensure_ascii=False)

    # Summary
    print(f"\nBuild complete: {OUTPUT_PATH}")
    print(f"  Mathematical reasoning: {len(math_questions)} questions")
    print(f"  Thinking skills:        {len(thinking_questions)} questions")
    print(f"  Reading passages:       {len(reading_passages)} passages")
    print(f"  Reading questions (MCQ): {len(reading_questions)} questions")
    total = len(math_questions) + len(thinking_questions) + len(reading_questions)
    print(f"  Total questions:        {total}")


if __name__ == "__main__":
    build()
