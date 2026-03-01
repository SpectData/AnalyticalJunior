#!/usr/bin/env node
/**
 * Generate heart sprites for the HUD lives display using DALL-E 3.
 * Outputs 2 PNGs to UnityProject/BrainAcademy/Assets/Sprites/
 *
 * Usage: node scripts/generate_hearts.mjs
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

const STYLE =
  "2D pixel-art game icon, 64x64 pixels, transparent background, clean outlines, cartoon style inspired by Plants vs Zombies. No text or labels.";

const SPRITES = [
  {
    name: "heart_filled",
    prompt: `${STYLE} A bright red heart icon, fully filled, glossy and vibrant. Classic video game heart/life icon. Bright cherry red color with a small white highlight/shine on the upper left. Simple, bold, instantly recognizable as a life/health heart.`,
  },
  {
    name: "heart_empty",
    prompt: `${STYLE} A grey empty heart outline icon, hollow/unfilled. Classic video game lost-life heart. Dark grey outline with transparent/empty interior. Same shape as a standard heart icon but clearly shows the life has been lost. Dim and faded appearance.`,
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
    throw new Error(
      `DALL-E API error for ${sprite.name}: ${response.status} ${err}`
    );
  }

  const data = await response.json();
  const b64 = data.data[0].b64_json;
  const buffer = Buffer.from(b64, "base64");

  const outPath = path.join(SPRITES_DIR, `${sprite.name}.png`);
  fs.writeFileSync(outPath, buffer);
  console.log(
    `  Saved ${outPath} (${(buffer.length / 1024).toFixed(1)} KB)`
  );
}

async function main() {
  fs.mkdirSync(SPRITES_DIR, { recursive: true });

  console.log(`Generating ${SPRITES.length} heart sprites via DALL-E 3...\n`);

  for (const sprite of SPRITES) {
    try {
      await generateSprite(sprite);
    } catch (err) {
      console.error(`  Failed: ${sprite.name} - ${err.message}`);
    }
  }

  console.log("\nDone!");
}

main();
