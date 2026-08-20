# Deploy — runbook

How IPFees ships to production. The server side of this already exists and is out of scope here — see
the `webserver-deploy` repo, `Contabo - Valer/ipfees.dataman.ro/`, for the `docker-compose.yml` and `.env`
that are already running on the VM. This document is about the CI/CD wiring only.

## How it works

Push to `master` (or trigger manually) and
[`.github/workflows/deploy-app.yml`](../.github/workflows/deploy-app.yml):

1. Builds the `IPFees.API` and `IPFees.Web` Docker images.
2. Pushes them to GHCR (`ghcr.io/vbocan/ipfees:api` / `:web`, plus a `${{ github.sha }}`-free `latest`
   tag) and to Docker Hub (`vbocan/ipfees:api`, `:api-latest`, `:web`, `:web-latest`).
3. SSHes into the Contabo VM and, in `~/ipfees.dataman.ro`, runs `docker compose pull`, `docker compose
   up -d`, then `docker image prune -f`.

Same pattern as `deploy-app.yml` in gooblen.ro and caravana-medicala, adapted for two images (API + Web)
instead of one.

## One-time setup

- [ ] **Repo secrets** — `CONTABO_HOST`, `CONTABO_USER`, `CONTABO_SSH_KEY` (`DOCKERHUB_TOKEN` already
      exists). Point these at whichever Contabo VM currently runs `ipfees.dataman.ro`; the secret *names*
      match gooblen.ro/caravana-medicala, not necessarily the *values* — confirm whether this is the same
      VM before assuming the values carry over.
      ```bash
      gh secret set CONTABO_HOST --body "<ip-or-hostname>"
      gh secret set CONTABO_USER --body "<ssh-user>"
      gh secret set CONTABO_SSH_KEY < path/to/private_key
      ```
- [ ] **GHCR package visibility** — if `ghcr.io/vbocan/ipfees` is private, `docker login ghcr.io` needs to
      already be done on the server (a PAT with `read:packages` scope is enough). If the package is
      public, there's nothing to do here.
- [ ] Confirm `~/ipfees.dataman.ro/docker-compose.yml` on the server still matches
      `webserver-deploy/Contabo - Valer/ipfees.dataman.ro/docker-compose.yml` — this workflow doesn't
      touch it, it only runs `pull`/`up -d` against whatever is already there.

## First deploy after wiring secrets

Either push to `master`, or run it on demand: **Actions → deploy-app → Run workflow**.

## Rollback

There's no automatic rollback — a deploy is always "whatever `master` built most recently." To roll back
manually, SSH in and pin an older image by digest in `docker-compose.yml`, or re-run the workflow from an
earlier commit via `workflow_dispatch` on that ref:

```bash
cd ~/ipfees.dataman.ro
# edit docker-compose.yml to pin ghcr.io/vbocan/ipfees:api@sha256:<digest> if needed
docker compose pull
docker compose up -d
```
