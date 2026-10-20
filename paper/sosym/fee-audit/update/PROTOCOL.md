# Bringing the IPFees corpus to the fees in force (October 2026)

Every schedule of the reference corpus (`ipfees`, `src/IPFLang.Engine/Corpus/PCT-XX.ipf`, 118
files) is compared fee by fee with the official fees in force on 20 October 2026 and edited to
carry them. The corpus before this pass is IPFees `beab6fd`, whose amounts are those of the
January 2024 baseline. The work is split into ten batches of about twelve offices; each batch is
done by one research assistant, which edits only its own schedules and writes one record,
`batch-NN.md`, in this folder.

## Sources

1. The office's own current fee table, where it can be reached.
2. The WIPO PCT Applicant's Guide national chapter of the office (summary and Annex XX.I),
   already saved as plain text in `sources/XX.txt`; its first line gives the date from which the
   chapter applies ("Valid as from ..."). If the file is missing, run
   `python fetch_eguide.py /d/Repositories/ipfees/src/IPFLang.Engine/Corpus sources XX`.
3. For the fifteen offices audited on 19 October 2026 (US, EP, JP, CN, KR, IN, CA, AU, BR, MX, GB,
   IL, RO, CH, SG), `../AUDIT.md` already records the fees in force with their sources; use it,
   and confirm it against the saved chapter. Its last section gives the rows as they read at
   `beab6fd`, which is the baseline here.

Numbers are read from saved text, never from a summary. Save every page used other than the
eGuide chapter with `python fetch_page.py URL sources/XX-office.txt` (or `XX-office-2.txt`, ...);
it handles PDFs. Do not use a summarising web-fetch tool for any figure. A web search to find an
office's fee page is fine. Every figure written into a schedule must appear in a saved source.

When the office's table and the eGuide chapter disagree, prefer the more recent of the two and
say so. When an amount cannot be established (site unreachable, chapter older than the baseline
and in conflict with it, ambiguous wording), leave the schedule unchanged and record the row as
UNVERIFIED with the reason. A fee change announced for a later date (for example 1 January 2027)
is not applied; record it in the notes.

## What may change in a schedule

- The amounts and per-unit rates of the official fees the schedule already models.
- Their thresholds (claims, sheets, pages, priorities) and conditions, when the office's rule
  says otherwise today. Keep the explicit zero case consistent with a changed threshold
  (`YIELD 0 IF ClaimCount LTE 15` when the charge now starts at the 16th claim), so the
  `VERIFY COMPLETE` and `VERIFY MONOTONIC` directives still hold.
- A modelled fee the office no longer charges is removed with its `VERIFY` directives.
- Where an office distinguishes electronic and paper filing, use the electronic rate, as the
  corpus does, unless the schedule already models both.
- Keep the schedule's currency. If the office now charges foreign applicants in another
  currency, record it and ask (in your final report) rather than converting.
- The version line becomes `VERSION '2026.10' EFFECTIVE <date> DESCRIPTION '<unchanged>'`, where
  the date is the day from which the amounts now in the schedule apply: the effective date of the
  office's current table where the office or the chapter states it, otherwise the chapter's
  "Valid as from" date. Say in the record which one was used.
- The comment `# Valid until ...` in the official-fees header, where present, becomes
  `# Checked against the fees in force on 20 October 2026 (<short source names>)`.

## What must not change

- No new fee, input, group, reduction or `LET` that the schedule does not already have. An
  official national-phase-entry fee or reduction the schedule does not model is recorded as
  MISSING and left out.
- `TranslationFee`, `AgentServiceFee`, every `DEFINE`, `GROUP`, `RETURN` line, and the files in
  `Corpus/bases/`.
- Any file outside the batch. Do not build, test or run the IPFees solution; the build and the
  checks on save are run once, after all batches, on the whole corpus.

IPFLang in brief: `COMPUTE FEE Name [OPTIONAL] ... ENDCOMPUTE`; `LET X AS 120`;
`YIELD expr [IF cond]`; `CASE cond AS ... ENDCASE`; comparisons `EQ NEQ GT GTE LT LTE`, `AND`,
`OR`, `IN`/`NIN` for multi-select lists; `CEIL`, `FLOOR`, `ROUND`; `120<GBP>` is an amount in
pounds, and a plain number is in the currency of the schedule's `RETURN Currency` line. A fee's
amount is the sum of the yields whose conditions hold.

## The record (`batch-NN.md`)

One section per office:

```
## XX: PCT-XX.ipf (CUR)

Sources: eGuide XX, valid as from <date> (sources/XX.txt); <office table, its date> (sources/XX-office.txt).
Version line: '2024.1' EFFECTIVE 2024-01-01 → '2026.10' EFFECTIVE <date> (<which date this is>).
Fees last changed: <date of the office's most recent fee change, or "not stated">.

| Fee | Condition | Baseline | In force | Effective | Verdict | Source |
|---|---|---|---|---|---|---|
| OFF_BasicNationalFee | always | 200 | 210 | 1 Sep 2025 | UPDATED (amount) | XX-office "PF37 ... $210" |
| ... | | | | | MISSING | ... |

Notes: anything a reader of the schedule should know (announced changes, simplified conditions,
doubts).
```

A row is one fee under one condition; the entity-size variants of the same fee and condition
share a row (`320/128/64`). Translation and agent fees and the explicit zero cases are not rows.
Verdicts:

- `MATCH`: the baseline equals the fee in force; nothing changed.
- `UPDATED (amount|threshold|condition|basis)`: the schedule now carries the fee in force. Give in
  the Effective column the date the fee in force took effect, if known; when that date is before
  1 January 2024, the baseline was already out of date then, and the row says so.
- `REMOVED`: the office no longer charges the fee; the fee was deleted.
- `UNVERIFIED`: the fee in force could not be established; unchanged; reason in the row.
- `MISSING`: an official national-phase-entry fee or reduction the schedule does not model.

The Source column quotes the few words of the saved source that carry the figure, with the file
name.

End the record with one summary table for the batch:

```
| Office | Rows | Match | Updated | Removed | Unverified | Missing | Effective date used | Fees last changed |
```
