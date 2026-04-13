---
description: "SECURITY: Environment files containing secrets. Never read, reference, modify, or generate .env files."
applyTo: "**/.env,**/.env.*,**/*.env"
---

# Environment Files - DO NOT ACCESS

**These files contain secrets and must never be touched.**

## Forbidden Actions

- Do not read `.env` file contents
- Do not modify or overwrite `.env` files
- Do not reference actual values from `.env` files in responses
- Do not generate new `.env` files with real credentials

## Allowed Actions

- Reference `.env.example` files (templates with placeholder values)
- Explain how to configure environment variables conceptually
- Show example `.env` structure with dummy values like `YOUR_API_KEY_HERE`
