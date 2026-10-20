# Evaluation material for the SoSyM tool paper

V. Bocan, "IPFees: model-driven quotation of patent filing costs across jurisdictions",
submitted to *Software and Systems Modeling* as a Tool Paper (2026). The paper describes IPFees
release 1.1.0; this folder, in the same release, holds what its evaluation rests on.

| Folder | Paper | Content |
|---|---|---|
| `quotation/` | Section 6.1, Table 2 | The twenty-office quotation, the quotation over all 118 offices, the explanation with alternatives and the scaling series, measured through the REST service of a local deployment; scripts and raw results. |
| `fee-audit/` | Section 6.2, Table 3 | The comparison of every schedule with the fees in force on 20 October 2026: protocol, per-office records with the source quote behind each row, the independent checks, the tally, and the saved source pages (`update/sources/`; third-party commentary is listed in `NOT-INCLUDED.md` rather than copied). `AUDIT.md` is the fifteen-office pilot of 19 October 2026. |
| `RESULTS.md`, `harness/`, `results/`, `scripts/` | Section 5.7 | The cost of the checks on save (E3), and the seeded-fault study (E2) that the paper no longer reports. |
| `dmn/` | Section 8.3 | The running example as DMN decision tables, executed with Drools (Apache KIE 10). |

Each folder's README says how its results were produced and how to reproduce them. The quotation
and E3 measurements were taken on 20 October 2026 on one machine (Intel Core i9-13900K, 32 logical
processors, Windows 11), against the application code and corpus of this release.

The schedules themselves, the corpus the comparison concerns, are in
`src/IPFLang.Engine/Corpus/` of this repository.
