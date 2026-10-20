# E2–E3: save-gate detection and verification cost

Since the rewrite of 2026-10-19 (`REFRAMING_SoSyM.md`) the paper no longer reports E2, the
seeded-fault study; of E3 it keeps only the time the checks on save take (Section 5.7, from the
latest re-run below: median 0.61 ms, at most 88 ms warm). Section 6 now rests on `quotation/` and `fee-audit/`. The results
below stand as recorded.

**E3 re-run at `68b21e4` (IPFees v1.1.0), 2026-10-20.** Run on the machine of the quotation
measurements (`quotation/README.md`): Intel Core i9-13900K, 32 logical processors, Windows 11,
.NET 10.0.11, with `PLATFORM_REV=68b21e4 bash scripts/prepare.sh`, then `e3`; `results/e3/` now
holds this run. Since `012d5a9` only the Mongolian, Malawian and Namibian schedules and the version
lines changed. All 348 directives finish within 5 s and all hold (285 `VERIFY COMPLETE`, 63
`VERIFY MONOTONIC`); all 118 schedules are saved with every directive verified. The gate takes
0.61 ms per schedule warm (median; slowest CN, 88.0 ms). Section 5.7 quotes this run. The paragraph
below describes the run it replaces, on the AMD machine of the earlier runs.

**E3 re-run at `012d5a9`, 2026-10-20.** The corpus now carries the fees in force in October 2026
(`fee-audit/update/`), with eight monotonicity directives pointed at the input their fee reads
and Cuba's claim fee checked against each of its three counts. E3 was run at `012d5a9` on the same
machine as the run it replaced (`PLATFORM_REV=012d5a9 bash scripts/prepare.sh`, then `e3`). All 348 directives finish within 5 s and all hold: 285 `VERIFY COMPLETE` and 63
`VERIFY MONOTONIC`. All 118 schedules are saved with every directive verified. The gate takes
0.63 ms per schedule warm (median; slowest CN, 128.9 ms, whose waiver now depends on both the
receiving office and the searching authority) and 40.3 ms on a cold first call (median; max
351.6 ms); the 45 schedules with `VERIFY MONOTONIC` take 4.2 ms warm (median). Section 5.7 quoted
this run until the re-run above. The paragraph below describes the run it replaced.

**E3 re-run, 2026-10-20.** IPFees now carries the 118 schedules whose expert validation the
IPFLang article reports (`cea944f` removes Malaysia, which upstream added afterwards), and
`beab6fd` corrects the audit's four modelling errors and the CN, PH and OA open cases. E3 was run
again at `beab6fd` on an AMD Ryzen 9 7900 (24 logical processors; Windows 11; .NET 10.0.13); its
files in `results/e3/` were replaced by the run above (`PLATFORM_REV=beab6fd bash scripts/prepare.sh`, then `e3`). All 349
directives finish within 5 s and all hold: 287 `VERIFY COMPLETE` and 62 `VERIFY MONOTONIC`. All 118
schedules are saved with every directive verified. The gate took 0.62 ms per schedule warm
(median; slowest US, 75.8 ms) and 39.2 ms on a cold first call (max 113.3 ms); the 45 schedules
with `VERIFY MONOTONIC` took 4.3 ms warm (median). A run at `cea944f`
the same morning, with CN still refused, gave 0.64 ms and 83.5 ms. Everything below,
E2 included, stands as recorded for the 119-schedule corpus and its own commits; its E3 tables no
longer match the files in `results/e3/`.

Evidence for Section 6 of the earlier, gate-centred version of the SoSyM tool paper (OUTLINE.md,
"Evidence to gather").
Measured 2026-10-19 on one machine (Intel Core i9-13900K, 32 logical processors; Windows 11;
.NET 10.0.11). An earlier run at `be68c7c`, on an AMD Zen 4 machine (24 logical processors,
.NET 10.0.13), gave the same verdict for every schedule, directive and mutant and the same gate
messages; only the timings differ.

**What was measured.** IPFees at ipfees commit `50868e2` (master), which carries
the IPFLang engine (version 1.5.0) and its jurisdiction corpus as source in `src/IPFLang.Engine`:
119 schedules and 4 regional bases. Schedules are composed through the platform's
`FeeScriptComposer` and gated by its `FeeDefinitionValidator`, which runs `ScheduleVerifier`
under the production 5 s `VerificationBudget`; the fee pages store a definition only when
`MayBeSaved` holds. The harness builds the engine project and compiles those platform classes,
and `FeeCurrency` for the currency oracle, from `50868e2` itself. Nothing in the repository was
modified; `scripts/prepare.sh` streams `src/IPFLang.Engine` and `src/IPFees.Core` out with
`git archive`.

The paper describes ipfees `428abd9`. The four commits after `50868e2` change what the harness
does not exercise: `2226c9d` the counterfactual totals of the explain API, `cd84d71` seeding through
the save gate and module deletion through the dependent check (with sign-in for authoring),
`478dbb8` the removal of that sign-in, and `428abd9` a reset that also removes what the corpus does
not define. The validator gains a method for module removal but its `ValidateAsync` path, which E2
and E3 run, is unchanged, so these results stand for `428abd9`. One
consequence of `cd84d71` for the figures below: an instance no longer serves the Chinese schedule,
which the gate refuses (E3's verdict).

## Findings first

1. **Nothing reaches the budget, and the corpus passes its own gate except CN.** All 352
   directives finish. 286 of the 289 `VERIFY COMPLETE` directives hold; the three that fail are
   CN `OFF_BasicNationalFee`, `OFF_ClaimFee` and `OFF_SheetFee`, which charge nothing when the
   receiving office is the national office, so CN alone is refused unchanged. All 63
   `VERIFY MONOTONIC` directives hold. The gate takes 0.57 ms per schedule warm (median; slowest
   US, 68 ms) and 35 ms on a cold first call (max 119 ms). Across 3 308 mutants, checked by 12
   parallel workers, the slowest gate run took 205 ms, and no mutant left a directive unfinished.
2. **Completeness now sees branch faults, because the zero cases are written down.** With every
   "charges nothing" case stated as `YIELD 0 IF …`, deleting or narrowing a branch leaves inputs
   where no yield fires. The gate refuses 54 % of the branch deletions and 45 % of the guard
   narrowings that change an amount, and 89 % of the inversions. It also refuses 189 mutants that
   change no amount (edits to the zero-case lines themselves), so the table gives both rates.
3. **Monotonicity is mostly caught through completeness; the checker itself samples.** 87 mutants
   make a fee break its declared direction (execution oracle). The gate refuses 63 of them, 54 of
   those through a completeness gap; the monotonicity checker catches 9 directly. It evaluates the
   target at about 20 evenly spaced points (for `ClaimCount` 1..1000: 1, 53, 105, …, 1000), so a
   fault confined to one band goes unseen when the fee has no `VERIFY COMPLETE`. MY
   `OFF_ClaimFee` is the case in point: it is priced in four claim bands and declares only
   `VERIFY MONOTONIC`, and deleting, narrowing or inverting any band boundary is saved (14
   mutants).
4. **A fee in two currencies is refused; a fee in the wrong currency is not.** Calculation
   converts each fee from its own currency (`FeeCurrency.Resolve`), and the validator now resolves
   every fee's currency the same way, refusing one whose yields name two currencies
   (`[MixedFeeCurrencies]`). That catches all 47 seeded currency changes that leave a fee mixed,
   in 13 schedules (e.g. one AE yield `2000<AED>` → `2000<EUR>` beside other AED yields), which
   would otherwise make every calculation for the jurisdiction fail. The other 276 currency
   changes leave a fee consistently in another currency (the agent or translation fee in USD
   instead of EUR, say). They are saved and change the amount production converts, and no check
   can see them without knowing which currency the office charges in.

## E2 — what the save gate catches

3 308 mutants, each one site of one jurisdiction script (the text an author edits; the regional
bases hold no fees), sent through `FeeDefinitionValidator.ValidateAsync` with the 5 s budget and
judged by the fee pages' rule (`!MayBeSaved`: refuse on a parse or type error, or on any directive
that finished and failed). A mutant counts as **detected** when the gate refuses it with a finding
the unmutated schedule does not have: a parse or type error, or a failing directive on a fee whose
directive holds on the original. This counts every mutant, the 48 of CN included. An execution
oracle runs alongside: the mutated fee is evaluated on pairwise input vectors built from the
schedule's list choices, booleans, defaults, bounds and guard literals ±1 (145–274 vectors per
schedule). An amount changed on any vector makes the mutant *observable*; a step against a declared
`VERIFY MONOTONIC` direction under an input sweep marks it *monotonicity violated*; and
`FeeCurrency.Resolve` on original and mutant says whether production would convert the fee from a
different currency, or fail to.

| Operator | Mutants | Detected | Rate | Detected by | Observable | Observable detected | Rate | Saved | Saved, unfinished | Monotonicity violated (caught) |
|---|---|---|---|---|---|---|---|---|---|---|
| Delete a `CASE` branch | 33 | 21 | 63.6 % | completeness 21 | 26 | 14 | 53.8 % | 9 | 0 | 0 |
| Delete a conditional `YIELD` ¹ | 408 | 251 | 61.5 % | completeness 251 | 344 | 187 | 54.4 % | 152 | 0 | 16 (12) |
| Narrow a guard by one unit | 296 | 166 | 56.1 % | completeness 166 | 234 | 105 | 44.9 % | 126 | 0 | 31 (23) |
| Change a literal's currency | 326 | 47 | 14.4 % | currency resolution 47 | 0 ² | — | — | 276 | 0 | 0 |
| Mix two currencies in one `YIELD` | 867 | 867 | 100 % | type checker 867 ⁴ | 867 | 867 | 100 % | 0 | 0 | 2 (2) |
| Invert a comparison in a fee with `VERIFY MONOTONIC` | 216 | 199 | 92.1 % | completeness 195, monotonicity 5 | 159 | 142 | 89.3 % | 11 | 0 | 30 (23) |
| Change a constant (+1) | 1 162 | 3 | 0.3 % | monotonicity 3 | 1 160 | 3 | 0.3 % | 1 143 | 0 | 8 (3) |
| **All** | **3 308** | **1 554** | **47.0 %** | type checker 867, currency resolution 47, completeness 633, monotonicity 9 ³ | 2 790 | 1 318 | 47.2 % | 1 717 | **0** | 87 (63) |

¹ The corpus writes branches mostly as `YIELD … IF …` (408) rather than `CASE` blocks (33), so
"delete a branch" is reported for both forms.
² No computed amount changes, because a currency literal evaluates to its number; but all 326
change the currency `FeeCurrency` resolves, 47 of them to an unresolvable mix, which currency
resolution refuses (finding 4).
³ Two mutants fail both a completeness and a monotonicity directive. The remaining 37 refused
mutants, all in CN, show only CN's existing gaps.
⁴ Currency resolution also fires on all 867; they are credited to the engine's type checker, which
refuses them first. "Currency resolution" counts only refusals it is alone in making.

Operator definitions: narrowing maps `GT n`→`GT n+1`, `GTE n`→`GT n`, `LT n`→`LT n−1`,
`LTE n`→`LT n`; a currency change swaps EUR↔USD (others to EUR); mixing appends
`+ 1<C> + 1<D>` to the yield, with `C` its own or the schedule's currency; inversion negates
(`GT`↔`LTE`, `EQ`↔`NEQ`, …) every comparison in a fee that carries `VERIFY MONOTONIC`; the
constant operator adds 1 to each numeric literal of a `LET` or yield value.

What the table says:

- **Without the mixed-currency operator** the gate detects 687 of 2 441 mutants (28.1 %), and 451
  of the 1 923 that change an amount (23.5 %). The type checker still catches every
  mixed-currency yield.
- **What completeness still cannot see** is a fault inside a region some other yield still
  covers: a changed rate or offset, or a branch whose removal leaves a base amount in place. That
  is why a third to a half of the branch faults that change an amount are saved.
- **Constant changes pass, as expected.** 3 of 1 162 are detected, all reduction rates raised
  above 100 % so that the fee falls as claims rise; the rest is what the practitioner check
  against official tables has to cover.
- 898 of the 1 717 saved mutants sit in fees with no `VERIFY` directive at all (agent and
  translation fees among them), where only the type checker and currency resolution can act.

## E3 — what verification costs

Each schedule runs through the production validator in a fresh process with the 5 s budget, once
cold and five times warm, and then `ScheduleVerifier` alone five times (`e3/schedules.csv`). Each
`VERIFY` directive then runs alone through `ScheduleVerifier` in a fresh process
(`e3/directives.csv`). Processes run one at a time.

| Gate verdict on the shipped schedule | Schedules |
|---|---|
| Saved, every directive verified | 118 |
| Refused, a completeness directive fails (CN) | 1 |
| Saved with directives unfinished at 5 s | 0 |

| | Completeness (289 directives) | Monotonicity (63 directives) |
|---|---|---|
| Finished within 5 s | 289 | 63 |
| Verdict | 286 complete, 3 incomplete (CN) | 63 monotonic |
| Time alone, cold process | median 6.7 ms, max 48 ms | median 13 ms, max 49 ms |
| Space examined | only the inputs the fee's guards reference: 150 unconditional, 135 exhaustive (median 600, max 100 000 combinations), 4 sampled (above 10^6) | the target plus the inputs the fee reads, at representative values: 0 other inputs for 40 directives, 1 for 20, 2 for 2, 3 for 1; 21 to 3 150 fee evaluations |
| Same checks through the engine's own entry point (`DslCalculator.RunVerifications`, every declared input) | — | 15 other inputs, 10^9.75 to 10^10.95 contexts: 1.2 × 10^11 to 1.9 × 10^12 planned fee evaluations; 0 of 63 finish within 5 s (`engine_*` columns of `e3/directives.csv`) |

| Time per schedule | Median | Max |
|---|---|---|
| Gate, warm (compose, type-check, resolve currencies, verify) | 0.57 ms | 68 ms (US) |
| Gate, first call in a process (JIT included) | 35 ms | 119 ms |
| `ScheduleVerifier` alone, warm | 0.08 ms | 65 ms |
| Gate, schedules with `VERIFY MONOTONIC` (46), warm | 4.3 ms | 68 ms |

A schedule's full declared input space is 10^32.8 to 10^34.0 combinations (16 inputs each,
dominated by word, sheet and claim counts), but neither checker enumerates it: completeness looks
only at the inputs a fee's guards reference, and monotonicity only at the inputs the fee reads, so
the largest plan is 3 150 fee evaluations. The budget is never approached: the slowest run, a cold
first call, uses under 3 % of it.

## Re-running

Needs git, bash and the .NET 10 SDK; run here with SDK 10.0.303 on Windows 11 under Git Bash.
The harness has no platform-specific code, but it has not been run in the
`mcr.microsoft.com/dotnet/sdk` image.

```bash
bash "PREPARING - Springer SoSyM - IPFees/evaluation/scripts/run_all.sh"
```

or step by step, from `evaluation/`:

```bash
bash scripts/prepare.sh                      # extract ipfees 50868e2 (src/IPFLang.Engine, src/IPFees.Core) into work/
dotnet build harness/Harness.csproj -c Release
dotnet harness/bin/Release/net10.0/Harness.dll e3                  # a few minutes, run alone
dotnet harness/bin/Release/net10.0/Harness.dll e2 --workers=12     # under 1 min
```

`prepare.sh` reads the ipfees checkout from `$IPFEES_REPO` (default: the checkout `paper/sosym` sits in) and
the commit from `$PLATFORM_REV` (default `v1.1.0`). Each command takes jurisdiction codes to
restrict it, e.g. `e2 RO US`; `e2-count RO` lists the mutants of a schedule without running them.
E2's oracle vectors generate dates relative to the run day; everything else is deterministic apart
from timings.

| Path | Content |
|---|---|
| `harness/` | the C# harness (`E2/`, `E3/`, shared corpus access, vector generator and worker plumbing in `Common/`) |
| `results/e2/` | `detection.csv` (per operator), `mutants.csv` (every mutant), `baselines.csv` (the unmutated schedules) |
| `results/e3/` | `schedules.csv`, `directives.csv`, `environment.json` |
