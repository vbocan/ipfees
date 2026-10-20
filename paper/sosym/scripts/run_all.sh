#!/usr/bin/env bash
# Re-run E2 and E3 from scratch: extract the pinned IPFees sources, build the harness, run
# each experiment. Results land in evaluation/results/{e2,e3}/.
#
# E3 runs first and alone: it times verification, so nothing else should compete for the
# CPU. E2 runs its mutants in parallel worker processes (--workers, default 12); every gate
# run that finishes does so in well under a second, so contention cannot move a verdict across
# the 5 s budget.
set -euo pipefail

HERE="$(cd "$(dirname "${BASH_SOURCE[0]}")/.." && pwd)"
bash "$HERE/scripts/prepare.sh"
dotnet build "$HERE/harness/Harness.csproj" -c Release

H=(dotnet "$HERE/harness/bin/Release/net10.0/Harness.dll")
"${H[@]}" e3
"${H[@]}" e2 --workers="${E2_WORKERS:-12}"
