#!/usr/bin/env python3
"""Re-generate wizard as a single character with explicit dark background."""

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


def main():
    # Generate with a very explicit single-character prompt
    print("Generating wizard (attempt with tighter prompt)...", flush=True)
    resp = requests.post(
        "https://api.openai.com/v1/images/generations",
        headers={
            "Authorization": f"Bearer {API_KEY}",
            "Content-Type": "application/json",
        },
        json={
            "model": "dall-e-3",
            "prompt": (
                "A single pixel-art wizard character for a mobile game, centered in the frame. "
                "Blue pointy hat, blue robe, holding a glowing golden wand in right hand. "
                "Facing right in side-view profile. Chibi proportions (big head, small body). "
                "64x64 pixel art style with clean black outlines. "
                "Solid dark navy blue (#000033) background filling the entire image. "
                "IMPORTANT: Only ONE character. No duplicates, no reflections, no variations. "
                "No text, no labels, no color swatches, no UI."
            ),
            "n": 1,
            "size": "1024x1024",
            "response_format": "b64_json",
        },
        timeout=60,
    )
    resp.raise_for_status()
    data = resp.json()
    raw = base64.b64decode(data["data"][0]["b64_json"])
    out = SPRITES_DIR / "wizard.png"
    out.write_bytes(raw)
    print(f"  Generated ({len(raw) // 1024} KB)")

    # Remove background
    print("  Removing background...", flush=True)
    img = Image.open(out)
    result = remove(img)

    # Auto-crop to content (trim transparent edges)
    bbox = result.getbbox()
    if bbox:
        result = result.crop(bbox)

    # Pad to square with transparent bg
    w, h = result.size
    side = max(w, h) + 20  # 10px padding each side
    final = Image.new("RGBA", (side, side), (0, 0, 0, 0))
    paste_x = (side - w) // 2
    paste_y = (side - h) // 2
    final.paste(result, (paste_x, paste_y))

    final.save(out)
    print(f"  Saved wizard.png ({out.stat().st_size // 1024} KB)")


if __name__ == "__main__":
    main()
