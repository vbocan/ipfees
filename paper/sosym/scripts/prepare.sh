#!/usr/bin/env bash
# Extract the IPFees sources the harness compiles against into evaluation/work/platform, at
# the commit whose behaviour is measured: the IPFLang engine IPFees carries as source, with its
# embedded corpus (src/IPFLang.Engine), and the save gate (FeeDefinitionValidator,
# ScheduleVerifier, FeeScriptComposer), per-fee currency resolution (FeeCurrency) and the types
# they need (src/IPFees.Core). The repository is only read: git archive streams the tree out,
# nothing is checked out.
set -euo pipefail

HERE="$(cd "$(dirname "${BASH_SOURCE[0]}")/.." && pwd)"
IPFEES="${IPFEES_REPO:-$HERE/../..}"   # the ipfees checkout this folder sits in
PLATFORM_REV="${PLATFORM_REV:-v1.1.0}"

rm -rf "$HERE/work"
mkdir -p "$HERE/work/platform"

git -C "$IPFEES" archive "$PLATFORM_REV" src/IPFLang.Engine src/IPFees.Core \
  | tar -x -C "$HERE/work/platform"

echo "platform_rev=$(git -C "$IPFEES" rev-parse "$PLATFORM_REV")" > "$HERE/work/revisions.txt"
cat "$HERE/work/revisions.txt"
