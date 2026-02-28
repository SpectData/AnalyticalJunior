#!/usr/bin/env python3
"""
Regenerate broken sprites (wizard, snake_red, spell, grass_dark)
and remove backgrounds.
"""

import json
import math
from pathlib import Path

import requests
from PIL import Image, ImageDraw, ImageFilter
from rembg import remove

SPRITES_DIR = (
    Path(__file__).parent.parent
    / "UnityProject"
    / "BrainAcademy"
    / "Assets"
    / "Sprites"
)

API_KEY = __import__("os").environ["OPENAI_API_KEY"]
API_URL = "https://api.openai.com/v1/images/generations"

STYLE = (
    "2D pixel-art game sprite, 64x64 pixels, solid dark blue (#000066) background, "
    "clean outlines, cartoon style inspired by Plants vs Zombies. "
    "No text, no labels, no UI elements, no color palette. Single character only."
)


def generate_and_save(name: str, prompt: str) -> Path:
    """Generate a sprite via DALL-E 3 and save it."""
    print(f"  Generating {name}...", end=" ", flush=True)
    resp = requests.post(
        API_URL,
        headers={
            "Authorization": f"Bearer {API_KEY}",
            "Content-Type": "application/json",
        },
        json={
            "model": "dall-e-3",
            "prompt": prompt,
            "n": 1,
            "size": "1024x1024",
            "response_format": "b64_json",
        },
        timeout=60,
    )
    resp.raise_for_status()
    data = resp.json()
    import base64

    b64 = data["data"][0]["b64_json"]
    raw = base64.b64decode(b64)
    out = SPRITES_DIR / f"{name}.png"
    out.write_bytes(raw)
    print(f"saved ({len(raw) // 1024} KB)")
    return out


def remove_bg(path: Path) -> None:
    """Remove background using rembg."""
    print(f"  Removing bg: {path.name}...", end=" ", flush=True)
    img = Image.open(path)
    result = remove(img)
    result.save(path)
    print("done")


def create_spell_sprite() -> None:
    """Create a golden spell projectile programmatically — more reliable than DALL-E."""
    print("  Creating spell.png programmatically...", end=" ", flush=True)
    size = 256
    img = Image.new("RGBA", (size, size), (0, 0, 0, 0))
    draw = ImageDraw.Draw(img)
    cx, cy = size // 2, size // 2

    # Outer glow
    for r in range(80, 0, -2):
        alpha = int(80 * (1 - r / 80))
        draw.ellipse(
            [cx - r, cy - r, cx + r, cy + r],
            fill=(255, 215, 0, alpha),
        )

    # Core bright circle
    core_r = 30
    draw.ellipse(
        [cx - core_r, cy - core_r, cx + core_r, cy + core_r],
        fill=(255, 240, 150, 255),
    )

    # Inner white-hot center
    inner_r = 15
    draw.ellipse(
        [cx - inner_r, cy - inner_r, cx + inner_r, cy + inner_r],
        fill=(255, 255, 230, 255),
    )

    # Add sparkle rays
    for angle_deg in range(0, 360, 45):
        angle = math.radians(angle_deg)
        length = 50 + (15 if angle_deg % 90 == 0 else 0)
        x1 = cx + int(20 * math.cos(angle))
        y1 = cy + int(20 * math.sin(angle))
        x2 = cx + int(length * math.cos(angle))
        y2 = cy + int(length * math.sin(angle))
        draw.line([x1, y1, x2, y2], fill=(255, 230, 100, 200), width=4)

    # Soft blur for glow effect
    img = img.filter(ImageFilter.GaussianBlur(radius=2))

    out = SPRITES_DIR / "spell.png"
    img.save(out)
    print(f"done ({out.stat().st_size // 1024} KB)")


def main() -> None:
    print(f"Fixing sprites in {SPRITES_DIR}\n")

    # 1. Regenerate wizard (single character, no duplicates)
    generate_and_save(
        "wizard",
        f"{STYLE} A single friendly young wizard character facing right, "
        "wearing a blue robe and pointy blue hat, holding a glowing wand. "
        "Side view profile. Bright cheerful pixel-art colors. "
        "Only ONE wizard, centered in the image.",
    )
    remove_bg(SPRITES_DIR / "wizard.png")

    # 2. Regenerate snake_red (single snake, no sprite sheet)
    generate_and_save(
        "snake_red",
        f"{STYLE} A single medium red snake enemy slithering to the left, "
        "cartoonish with small horns and slightly tough appearance. "
        "Bright red color. Only ONE snake, centered in the image. "
        "No sprite sheet, no multiple views.",
    )
    remove_bg(SPRITES_DIR / "snake_red.png")

    # 3. Regenerate grass_dark (clean tile)
    generate_and_save(
        "grass_dark",
        "2D pixel-art game tile, 64x64 pixels, top-down view. "
        "A simple dark green grass tile with subtle grass texture. "
        "Dark lawn green color (#4A9E23). Tileable square. "
        "No border, no frame, no UI elements. Just the grass filling the entire image.",
    )
    # Don't remove bg for grass — it's a background tile that should be opaque

    # 4. Create spell programmatically (DALL-E can't do bright glowing things well)
    create_spell_sprite()

    print("\nDone! Fixed 4 sprites.")


if __name__ == "__main__":
    main()
