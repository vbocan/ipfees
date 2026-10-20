# IPFLang.Engine in IPFees

This directory is the IPFLang engine and its reference corpus, carried as source inside IPFees.

- **Upstream:** https://github.com/vbocan/IPFLang, commit `e32d6cc` (released as `IPFLang.Engine`
  1.4.0 on nuget.org). Imported with `git archive` from `src/IPFLang.Engine`,
  `src/IPFLang.Engine.Tests` and `jurisdictions/`.
- **Licence:** GPL-3.0-or-later, as upstream.
- **Layout here:** engine source in `src/IPFLang.Engine`, its tests in `src/IPFLang.Engine.Tests`,
  the 118 schedules in `Corpus/` and the four regional bases in `Corpus/bases/`. The corpus is
  embedded under the same resource names as upstream, so `JurisdictionCorpus` is unchanged.
- **Version:** 1.5.0 (local).

## Changes made here

### Engine

- `Analysis/ConditionExtractor.cs`: membership conditions (`SYMBOL IN LIST`, `SYMBOL NIN LIST`)
  now take the multi-select list as the variable, as the evaluator does. The analysis used to
  take the symbol, found no input of that name, and reported any fee guarded by membership as
  having no yields. Tests: `CompletenessTests.CheckCompleteness_Membership*`.
- `Types/CurrencyTypeChecker.cs`, `Types/IPFType.cs`, `Types/TypeError.cs`: a name the checker
  cannot resolve is still typed as a string, so that list choice symbols compare as before, but
  it now keeps its name (`IPFTypeSymbol`). Used in arithmetic, inside `CEIL`, `FLOOR` or `ROUND`,
  compared with a number or an amount, or as a yield's whole value, it is reported as
  `[UndefinedVariable] 'ClaimCount' is not declared as an input or a LET variable`. The checker
  used to report "Cannot perform arithmetic on non-numeric type: String", and inside a function
  also a second error about the failed subexpression; that is what a member state's schedule was
  told when its regional base dropped an input it reads. Tests:
  `CurrencyTypeTests.TestTypeCheckerUndeclaredName*`, `TestTypeCheckerChoiceSymbolsRemainValid`.
- `Provenance/CounterfactualEngine.cs`: the variations a counterfactual is drawn from (each other
  choice of a list, a number's bounds and midpoint, a boolean's opposite, double and half an
  amount) are exposed as `GetCandidates`, each with the input values that result, and
  `GenerateCounterfactuals` draws from it, otherwise unchanged. `GenerateCounterfactuals` adds
  every fee as a bare number and keeps a variation only when that sum moves, which mixes
  currencies in a schedule that prices fees in more than one; IPFees computes the candidates
  itself and totals them per currency (`FeeCalculator.Explain`). Test:
  `ProvenanceTests.GetCandidates_KeepsVariationsThatLeaveTheTotalUnchanged`.

### Corpus

- **The validated set of jurisdictions.** Upstream added `PCT-MY` in `4ada1a3`, after the
  expert validation of 118 schedules that the IPFLang article reports. IPFees carries those
  118 and leaves Malaysia out, so that every schedule it ships is one that was validated.
- **Per-block charges round up.** Nine yields in AT, CU, DZ, IL, UA and US charge per started
  block of claims or sheets and now wrap the block count in `CEIL`.
- **Translation is priced in EUR.** `TranslationFee` read `12 * NumberOfWords / 100`, which by
  the language's rules is in the schedule's own currency (12 IDR, 12 JPY per hundred words). It
  now reads `12<EUR> * NumberOfWords / 100` in all 118 schedules.
- **Intended zero cases are explicit.** 67 fees that charge nothing up to a threshold or when a
  condition does not hold (no excess claims or sheets, no extension, no further priority, search
  already performed, validation country not chosen) declared `VERIFY COMPLETE` without stating
  that case, so the check failed on all of them. Each now states it, as `YIELD 0 IF <condition>`
  or a `CASE <condition> AS YIELD 0 ENDCASE` block.
- **EP supplementary search fee.** The full-fee line excluded `ISA_NONE` instead of `ISA_EPO`, so
  an application searched by the EPO paid 0 + 1,460 EUR and one searched by an unlisted authority
  paid nothing. It now excludes `ISA_EPO`.

The corrections below fix how a fee is charged and keep the amounts of the corpus baseline
(January 2024). Each is pinned by `CorpusCorrectionsTests`.

- **EP examination fee after an EPO preliminary report.** Art. 14(2) RFees reduces the fee by
  75 % when the EPO drew up the international preliminary examination report; the schedule
  multiplied it by 0.75, charging 75 % instead of 25 %. The factor is now 0.25.
- **US multiple dependent claim fee.** 37 CFR 1.492(j) charges it once for an application that
  contains a multiple dependent claim; the schedule charged it per such claim.
- **CN.** CNIPA waives the filing fee and the application surcharges (claim and sheet fees) for an
  application it received as receiving office. The three fees priced nothing in that case without
  saying so, and the claim and sheet fees had no zero case below their thresholds either, so
  their completeness directives failed and the gate refused the schedule. Each now states those
  cases. The examination fee is also waived when CNIPA drew up the international search report,
  not only the preliminary report, and the 20 % reduction for an EPO, JPO or Swedish search no
  longer combines with the waiver into a negative amount.
- **BR annuities.** The schedule charged the annuities of the first and second years at entry,
  and the first of them unconditionally. None falls due before the 24-month point from the
  international filing date, so only the third-year annuity remains.
- **PH sheet fee.** IPOPHL charges per sheet in excess of thirty; the schedule charged from the
  sixth sheet once the thirtieth was passed (`Fee * (SheetCount - 5)` above 30).
- **OA acceptance fee for description and drawings.** OAPI charges one flat fee for the band
  the sheets fall in (11 to 20, 21 to 30, 31 to 40) and above 40 sheets a further fee for each
  set of 10 sheets. The schedule multiplied each band's fee by the number of sheets in it, so 35
  sheets cost 7,200,000 XAF instead of 600,000, and it did not round the sets of 10 up.
