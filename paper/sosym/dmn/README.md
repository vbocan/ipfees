# The Swiss schedule in DMN (Section 8.3)

`PCT-CH.dmn` is the schedule of Listing 1 in DMN 1.5, and `base_ep.dmn` the part of the European
base it reads. Each fee is a decision; the claim fee is a decision table with hit policy UNIQUE and
one rule per yield. `RunDmn.java` evaluates the model with the Drools DMN engine of Apache KIE 10.0.0,
with runtime type checking on, for the running example (20 claims, 5,000 words) and for 8 claims,
and runs the validator's decision-table analysis. `output.txt` is its output of 20 October 2026.

```bash
MSYS_NO_PATHCONV=1 docker run --rm -v "$(pwd -W):/ws" -w /ws -v jbang-cache:/jbang \
  -e JBANG_DIR=/jbang jbangdev/jbang-action:0.119.0 RunDmn.java
```

What it shows, against the IPFLang schedule:

- **Amounts.** The decisions give the schedule's amounts: 200, 500 and 500 for the official fees,
  600 for translation and 100 for the agent.
- **Currency.** FEEL has numbers, not amounts. The item definitions `CHF` and `EUR` are both
  numbers, so they name a currency without enforcing one: the decision `Total`, typed `CHF`, adds
  the three franc fees to the two euro fees and the engine returns 1,900 with status SUCCEEDED,
  type checking included. IPFLang's type checker refuses `200<CHF> + 100<EUR>` when the schedule
  is loaded.
- **Composition.** The base's inputs are imported and referenced with the prefix `base.`
  (`base.ClaimCount`). An import adds the imported model's elements; nothing in the importing
  model can replace one of them, so the Swiss override of the base's currency has no counterpart.
  (Drools logs an import-resolution error on a first pass and then resolves the import; both models
  compile without messages.)
- **Explanation.** The engine's decision-table event names the rules that matched (rule 1 with 20
  claims, rule 2 with 8). The rule that did not apply, and the condition that excluded it, are left
  for the client to work out from the table.
- **What DMN gives.** The decision-table analysis checks the claim-fee table for gaps and overlaps
  and reports none, the counterpart of `VERIFY COMPLETE`. Monotonicity has no standard counterpart.
