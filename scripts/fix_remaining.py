#!/usr/bin/env python3
"""Fix wizard (crop to single character) and grass_dark (create programmatically)."""

import random
from pathlib import Path

from PIL import Image, ImageDraw

SPRITES_DIR = (
    Path(__file__).parent.parent
    / "UnityProject"
    / "BrainAcademy"
    / "Assets"
    / "Sprites"
)


def fix_wizard():
    """Crop wizard to just the left character (the cleaner one)."""
    print("  Fixing wizard — cropping to left character...", end=" ", flush=True)
    img = Image.open(SPRITES_DIR / "wizard.png")
    w, h = img.size
    # Take the left 50% of the image which has the main wizard
    cropped = img.crop((0, 0, w // 2, h))
    # Resize back to square
    cropped = cropped.resize((w, h), Image.LANCZOS)
    cropped.save(SPRITES_DIR / "wizard.png")
    print("done")


def create_grass_dark():
    """Create a dark grass tile programmatically — solid fill with subtle texture."""
    print("  Creating grass_dark.png programmatically...", end=" ", flush=True)
    size = 256
    base_color = (74, 158, 35)  # #4A9E23
    img = Image.new("RGBA", (size, size), (*base_color, 255))
    draw = ImageDraw.Draw(img)

    rng = random.Random(42)  # deterministic

    # Subtle darker patches
    for _ in range(200):
        x = rng.randint(0, size - 1)
        y = rng.randint(0, size - 1)
        shade = rng.randint(-20, 10)
        c = (
            max(0, min(255, base_color[0] + shade)),
            max(0, min(255, base_color[1] + shade)),
            max(0, min(255, base_color[2] + shade)),
            255,
        )
        s = rng.randint(2, 6)
        draw.rectangle([x, y, x + s, y + s], fill=c)

    # Tiny light grass blade highlights
    for _ in range(80):
        x = rng.randint(0, size - 1)
        y = rng.randint(0, size - 1)
        draw.rectangle([x, y, x + 1, y + 3], fill=(100, 180, 60, 255))

    img.save(SPRITES_DIR / "grass_dark.png")
    print("done")


def create_grass_light():
    """Create a light grass tile programmatically for consistency with dark."""
    print("  Creating grass_light.png programmatically...", end=" ", flush=True)
    size = 256
    base_color = (93, 191, 45)  # #5DBF2D
    img = Image.new("RGBA", (size, size), (*base_color, 255))
    draw = ImageDraw.Draw(img)

    rng = random.Random(99)  # deterministic, different seed

    # Subtle patches
    for _ in range(200):
        x = rng.randint(0, size - 1)
        y = rng.randint(0, size - 1)
        shade = rng.randint(-15, 15)
        c = (
            max(0, min(255, base_color[0] + shade)),
            max(0, min(255, base_color[1] + shade)),
            max(0, min(255, base_color[2] + shade)),
            255,
        )
        s = rng.randint(2, 6)
        draw.rectangle([x, y, x + s, y + s], fill=c)

    # Light highlights
    for _ in range(80):
        x = rng.randint(0, size - 1)
        y = rng.randint(0, size - 1)
        draw.rectangle([x, y, x + 1, y + 3], fill=(130, 210, 80, 255))

    img.save(SPRITES_DIR / "grass_light.png")
    print("done")


def main():
    fix_wizard()
    create_grass_dark()
    create_grass_light()
    print("\nAll fixes applied.")


if __name__ == "__main__":
    main()
