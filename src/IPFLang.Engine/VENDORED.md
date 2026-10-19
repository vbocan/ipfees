# IPFLang.Engine in IPFees

This directory is the IPFLang engine and its reference corpus, carried as source inside IPFees.

- **Upstream:** https://github.com/vbocan/IPFLang, commit `e32d6cc` (released as `IPFLang.Engine`
  1.4.0 on nuget.org). Imported with `git archive` from `src/IPFLang.Engine`,
  `src/IPFLang.Engine.Tests` and `jurisdictions/`.
- **Licence:** GPL-3.0-or-later, as upstream.
- **Layout here:** engine source in `src/IPFLang.Engine`, its tests in `src/IPFLang.Engine.Tests`,
  the 119 schedules in `Corpus/` and the four regional bases in `Corpus/bases/`. The corpus is
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

- **Per-block charges round up.** Nine yields in AT, CU, DZ, IL, UA and US charge per started
  block of claims or sheets and now wrap the block count in `CEIL`.
- **Translation is priced in EUR.** `TranslationFee` read `12 * NumberOfWords / 100`, which by
  the language's rules is in the schedule's own currency (12 IDR, 12 JPY per hundred words). It
  now reads `12<EUR> * NumberOfWords / 100` in all 119 schedules.
- **Intended zero cases are explicit.** 67 fees that charge nothing up to a threshold or when a
  condition does not hold (no excess claims or sheets, no extension, no further priority, search
  already performed, validation country not chosen) declared `VERIFY COMPLETE` without stating
  that case, so the check failed on all of them. Each now states it, as `YIELD 0 IF <condition>`
  or a `CASE <condition> AS YIELD 0 ENDCASE` block.
- **EP supplementary search fee.** The full-fee line excluded `ISA_NONE` instead of `ISA_EPO`, so
  an application searched by the EPO paid 0 + 1,460 EUR and one searched by an unlisted authority
  paid nothing. It now excludes `ISA_EPO`.

### Open in the corpus

- **CN** `OFF_BasicNationalFee`, `OFF_ClaimFee`, `OFF_SheetFee` are guarded on `RO_NP EQ FALSE`
  ("receiving office same as national phase office") and price nothing when it is `TRUE`.
  Whether that is right is for the schedule's author to confirm; the three completeness
  directives fail until it is settled.
- **PH** `OFF_SheetFee` charges `Fee * (SheetCount - 5)` only above 30 sheets; the offset and
  the threshold disagree.
- **OA** `Off_AcceptanceFee` charges `SF4 * ((SheetCount - 40) / 10)` above 40 sheets without
  rounding the block count up.
