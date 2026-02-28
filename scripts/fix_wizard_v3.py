#!/usr/bin/env python3
"""Last attempt at wizard - generate multiple and pick best, or use first gen's crop."""

import base64
import os
from pathlib import Path

import requests
from PIL import Image
from rembg import remove

SPRITES_DIR = (
    Path(__file__).parent.parent
    / "UnityProject"
    / "BrainAcademy"
    / "Assets"
    / "Sprites"
)

API_KEY = os.environ["OPENAI_API_KEY"]


def generate():
    resp = requests.post(
        "https://api.openai.com/v1/images/generations",
        headers={
            "Authorization": f"Bearer {API_KEY}",
            "Content-Type": "application/json",
        },
        json={
            "model": "dall-e-3",
            "prompt": (
                "Pixel art character sprite for a tower defense game. "
                "A cute chibi wizard boy with bright blue wizard hat and bright blue robe. "
                "He holds a magic wand with a golden star tip that is glowing. "
                "He faces to the right. Colorful, vibrant, happy expression. "
                "The background is solid black (#000000). "
                "Style: 16-bit retro game sprite, clean pixel edges, saturated colors."
            ),
            "n": 1,
            "size": "1024x1024",
            "response_format": "b64_json",
        },
        timeout=60,
    )
    resp.raise_for_status()
    return resp.json()


def process(raw_bytes: bytes, out: Path):
    """Remove bg, auto-crop, pad to square."""
    out.write_bytes(raw_bytes)
    img = Image.open(out)
    result = remove(img)

    bbox = result.getbbox()
    if bbox:
        result = result.crop(bbox)

    # If DALL-E gave us a wide image with duplicates, take the left half
    w, h = result.size
    if w > h * 1.5:
        result = result.crop((0, 0, w // 2, h))

    # Pad to square
    w, h = result.size
    side = max(w, h) + 20
    final = Image.new("RGBA", (side, side), (0, 0, 0, 0))
    final.paste(result, ((side - w) // 2, (side - h) // 2))
    final.save(out)


def main():
    out = SPRITES_DIR / "wizard.png"
    print("Generating wizard...", flush=True)
    data = generate()
    raw = base64.b64decode(data["data"][0]["b64_json"])
    print(f"  Generated ({len(raw) // 1024} KB), processing...")
    process(raw, out)
    print(f"  Saved ({out.stat().st_size // 1024} KB)")


if __name__ == "__main__":
    main()
