#!/usr/bin/env python3
"""Remove backgrounds from all sprite PNGs using rembg."""

from pathlib import Path

from PIL import Image
from rembg import remove

SPRITES_DIR = Path(__file__).parent.parent / "UnityProject" / "BrainAcademy" / "Assets" / "Sprites"


def main():
    pngs = sorted(SPRITES_DIR.glob("*.png"))
    print(f"Processing {len(pngs)} sprites in {SPRITES_DIR}\n")

    for png in pngs:
        print(f"  {png.name}...", end=" ", flush=True)
        img = Image.open(png)
        result = remove(img)
        result.save(png)
        print(f"done ({png.stat().st_size // 1024} KB)")

    print(f"\nAll {len(pngs)} sprites processed.")


if __name__ == "__main__":
    main()
