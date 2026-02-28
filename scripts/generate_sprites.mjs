#!/usr/bin/env node
/**
 * Generate PvZ-style sprites for Snake Spellcaster using DALL-E 3.
 * Outputs 8 PNGs to UnityProject/BrainAcademy/Assets/Sprites/
 *
 * Usage: node scripts/generate_sprites.mjs
 * Requires: OPENAI_API_KEY env var
 */

import fs from "fs";
import path from "path";
import { fileURLToPath } from "url";

const __dirname = path.dirname(fileURLToPath(import.meta.url));
const SPRITES_DIR = path.join(
  __dirname,
  "..",
  "UnityProject",
  "BrainAcademy",
  "Assets",
  "Sprites"
);

const OPENAI_KEY = process.env.OPENAI_API_KEY;
if (!OPENAI_KEY) {
  console.error("ERROR: OPENAI_API_KEY not set");
  process.exit(1);
}

// Shared style prefix for consistent art direction
const STYLE =
  "2D pixel-art game sprite, 64x64 pixels, transparent background, clean outlines, cartoon style inspired by Plants vs Zombies. No text or labels.";

const SPRITES = [
  {
    name: "wizard",
    prompt: `${STYLE} A friendly young wizard character facing right, wearing a blue robe and pointy hat, holding a glowing wand. Side view, ready to cast spells. Bright cheerful colors.`,
  },
  {
    name: "snake_green",
    prompt: `${STYLE} A small green snake enemy slithering left, cartoonish and slightly menacing with tiny fangs. Simple rounded body, bright green color.`,
  },
  {
    name: "snake_yellow",
    prompt: `${STYLE} A small yellow snake enemy slithering left, cartoonish with tiny fangs and slightly faster-looking than the green one. Bright yellow-gold color.`,
  },
  {
    name: "snake_red",
    prompt: `${STYLE} A medium red snake enemy slithering left, cartoonish but tougher-looking with small horns. Bright red color, slightly bigger than the green snake.`,
  },
  {
    name: "snake_purple",
    prompt: `${STYLE} A large purple snake boss enemy slithering left, cartoonish but intimidating with glowing eyes and a crown. Dark purple color, clearly the strongest snake.`,
  },
  {
    name: "spell",
    prompt: `${STYLE} A glowing golden magic projectile / energy bolt flying to the right, with sparkle trail. Small, bright, and flashy. Gold and white colors.`,
  },
  {
    name: "grass_light",
    prompt: `${STYLE} A simple light green grass tile, flat top-down view, bright lawn green color (#5DBF2D), subtle grass texture. Tileable square tile for a game battlefield grid.`,
  },
  {
    name: "grass_dark",
    prompt: `${STYLE} A simple dark green grass tile, flat top-down view, darker lawn green color (#4A9E23), subtle grass texture. Tileable square tile for a game battlefield grid, slightly darker than the light variant.`,
  },
];

async function generateSprite(sprite) {
  console.log(`  Generating ${sprite.name}...`);

  const response = await fetch(
    "https://api.openai.com/v1/images/generations",
    {
      method: "POST",
      headers: {
        Authorization: `Bearer ${OPENAI_KEY}`,
        "Content-Type": "application/json",
      },
      body: JSON.stringify({
        model: "dall-e-3",
        prompt: sprite.prompt,
        n: 1,
        size: "1024x1024",
        response_format: "b64_json",
      }),
    }
  );

  if (!response.ok) {
    const err = await response.text();
    throw new Error(`DALL-E API error for ${sprite.name}: ${response.status} ${err}`);
  }

  const data = await response.json();
  const b64 = data.data[0].b64_json;
  const buffer = Buffer.from(b64, "base64");

  const outPath = path.join(SPRITES_DIR, `${sprite.name}.png`);
  fs.writeFileSync(outPath, buffer);
  console.log(`  ✓ Saved ${outPath} (${(buffer.length / 1024).toFixed(1)} KB)`);
}

async function main() {
  // Ensure output directory exists
  fs.mkdirSync(SPRITES_DIR, { recursive: true });

  console.log(`Generating ${SPRITES.length} sprites via DALL-E 3...\n`);

  // Generate sequentially to avoid rate limits
  for (const sprite of SPRITES) {
    try {
      await generateSprite(sprite);
    } catch (err) {
      console.error(`  ✗ Failed: ${sprite.name} — ${err.message}`);
    }
  }

  // Summary
  const generated = fs
    .readdirSync(SPRITES_DIR)
    .filter((f) => f.endsWith(".png"));
  console.log(`\nDone! ${generated.length}/${SPRITES.length} sprites saved to ${SPRITES_DIR}`);
}

main();
