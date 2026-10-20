# Independent check of the October 2026 fee update: offices A to K

Checked on 20 October 2026 by reading `corpus-update.diff`, the records `batch-01.md` to
`batch-06.md` and `batch-10.md` (EP), the saved sources in `sources/`, and the schedules in
`ipfees/src/IPFLang.Engine/Corpus/` (read only). Scope: every office whose code starts with A to K.
That covers batches 01 to 05, plus EP (batch 10) and KR, KW and KZ (batch 06). Version lines,
comment-only lines, the ApplicationDate and SheetCount label changes and VERIFY changes were
ignored, as instructed.

Counting: one line per changed amount, rate, threshold, condition or currency in the diff. A
rewritten yield that changes both its amount and its threshold counts once.

## Changed values

AE AG AL AM AO AZ BA BB BN BW CH CR CU CZ DJ DO EC EE ES GH GM GT HN ID IN JM JO KE KG KH KN KP KW:
no amount, rate, threshold, condition or currency changed in the diff. IN changed only a comment
on the examination fee, and the new text ("31 months from the date of priority") matches IN.txt l.372.

- AP (7): OK. AP-office.txt (Harare Protocol 2025, Schedule I) gives "Designation fee per designated
  State 100", "Publication or republication 500", "each additional page from 31 to 100 pages 20" and
  "from 101 ... 30", "each additional claim from 11 to 50 claims 100" and "from 51 ... 200",
  "Request for substantive examination 1000" and "Grant 500". AP.txt agrees line by line. The
  two-yield tiers give 20/30 per page and 100/200 per claim, which is correct. Effective
  1 March 2025 (AP-office-2.txt l.78).
- AT (2): OK. Document fee 74: AT.txt "Document fee ( Schriftengebühr ) 74 EUR"; AT-office.txt
  "inkl. Schriftengebühr EUR74". The −20 electronic reduction: AT-office.txt "Ermäßigung bei
  elektronischer Einbringung des Antrags auf Einleitung EUR -20" (PCT national-phase section).
  The figures reconcile: 292 + 74 = 366, the office's paper amount.
- AU (3): OK. 400 / 350 / 550: AU.txt Annex "400 AUD (600 AUD)", "(a) ... 350 AUD", "(b) all other
  applications 550 AUD"; AU-office-2.txt "$370 $400", "$490 $550". These are the electronic rates.
- BG (5: 20.45, 35.79, 92.03, 10.23, currency BGN to EUR): the amounts are OK. BG-office-2.txt gives
  "1. Filing 20,45", "9. Publication ... 35,79", "7. Search and examination ... 92,03" and "Claiming
  priority - for each priority 10,23 each" (comma decimals, read correctly). BG.txt gives the same
  figures in EUR and "Entry into force: January 1, 2026" (BG-office.txt l.177). **The record
  disagrees with the schedule; see Discrepancies.**
- BH (3): OK. BH.txt says "The amount in parentheses is payable in case of filing by an individual",
  then "540 BHD (40 BHD)", "Publication fee 320 BHD (50 BHD)" and "2nd annual fee 200 BHD (100 BHD)".
  The renewal split by Entity_Person matches.
- BR (6): OK. BR-office-2.txt gives "Por meio eletrônico 260,00", "Até 10 (dez) reivindicações
  870,00", "R$ 140,00 ... 11ª a 15ª; de R$ 290,00 ... 16ª a 30ª; e de R$ 740,00 ... 31ª em diante"
  and "220 - no prazo ordinário 400,00". BR.txt "First annual fee ... 24 months ... 400 BRL" also
  matches. Electronic and full (not discounted) rates are used.
- BY (5): OK. BY.txt gives 110.25, 47.25, 535.50, 315 and 220.50 BYN. BY-office-2.txt rates (2.45,
  1.05, 11.9, 7, 4.9) × base value 45 (BY-office-3.txt "С 1 января 2026 г. ... 45 рублей") give
  the same figures.
- BZ (3: 450, 550, removal of the 1st-annuity yield): the amounts are OK. BZ-office.txt gives "On
  application for the grant of a Patent $450.00 (including 1st annuity fee)" and "For publication and
  grant of a patent $550.00"; BZ.txt agrees. **The removal is not recorded; see Discrepancies.**
- CA (8): OK. CA-office.txt (2026 list) gives "Basic national fee ... 241.24 595.06", "international
  search by the Commissioner ... 117.94 297.00", "In any other case ... 482.48 1,190.13" and
  "second, third and fourth anniversaries ... 60.26 134.02". The small and standard rates are in
  the right places.
- CN (4: three waiver conditions, removal of the 20 % ISA EP/JP/SE reduction): OK. CN-office.txt
  note 五 reads "由中国国家知识产权局作为受理局受理并进行国际检索的 ... 免缴申请费及申请附加费", with no
  reduction clause. CN.txt l.315: "exempted when ... (CNIPA) acts as the Receiving Office and the
  International Searching Authority". ISA_CN is a declared choice (PCT-CN.ipf l.99).
- CO (2): OK. CO-office.txt (Res. 6167/2026) gives "1.1.1.2.2.1 ... 118.000 | 146.000" and
  "1.1.1.2.2.5 ... 1.599.500 | 1.919.000". The dots are thousands separators and were read
  correctly. These are the online rates. CO.txt "Online 118,000 COP", "Online 1,599,500 COP".
- DE (1: Renewal3 threshold 18 to 24 months): the change is supported. DE.txt l.210 "due on the last
  day of the month containing the second anniversary (24 months)". **The record disagrees; see
  Discrepancies.**
- DK (5: 3600, 360, 660, years 1 and 2 moved to 24 months): the values are OK. DK-office-2.txt "a.
  Basic fee 3,000 3,600", "300 360", "1st/2nd/3rd fee year 550 660" (2026 column). DK.txt l.213: the
  first three fees are due at "the second anniversary (24 months)". **The record disagrees on the
  condition change; see Discrepancies.**
- DZ (3): OK, at the company rate. DZ-office.txt "762-01 ... Entreprise ... 15000", "762-03 ...
  10000", "762-21 ... chaque 5 pages supplémentaires au-delà des 10 premières ... 5000". DZ.txt has
  the same "— Company" amounts.
- EA (8: unitary 70000, examination 70000, 2nd independent claim 40000 written as a literal,
  each further independent claim 25000, claim fees 7000/7500/8000, grant 45000): OK.
  EA-office.txt l.112-115, 212-214 and 249: "70000", "7000 ... 7500 ... 8000", "70000 рублей и
  дополнительно 40000 ... по 25000", "45000". The 25 % ISR reduction is kept (l.130). EA.txt
  agrees.
- EG (1): OK. EG.txt l.290/352 "50,000 EGP". The date comes from EG-secondary.txt "The Decree took
  effect on 18 June 2025".
- EP (9: 720, 290, 720, 1595, 1365, 2240, 2010, 725, Renewal3 18 to 24 months): the amounts are OK.
  EP-office-2.txt items 3 "720", 15 "290" and "720", 2 "1 595", Art. 3 "reduced by EUR 1 365",
  6 "2 010" (supplementary search drawn up) and "2 240" (none drawn up), 4 "for the 3rd year 725";
  in force for payments "on or after 1 April 2026" (l.148). ExFee1/ExFee2 map correctly. The
  24-month threshold matches EP.txt l.410. **The record disagrees on the condition; see
  Discrepancies.**
- FI (4: 450, 60, 3rd year 140, new 2nd-year 80 from 12 months): OK. FI-office-2.txt "online filing
  system is used €450 (€430)", "each claim over 15 €60 (€50)", "2nd year €80 (€0)" and "3rd year
  €140 (€135)". FI.txt "220 EUR" = 80 + 140.
- GB (5: 40, 27, 13, 160, 200): OK. GB.txt "40 GBP", "27 GBP", "13 GBP"; search fee "200 GBP —
  ... requested electronically 160 GBP" and "240 GBP — ... electronically 200 GBP". The
  examination LET is unchanged at 130 (comment only), and GB.txt gives "130 GBP" electronic.
- GE (5: 0.8×100, OFF_SearchFee removed, 0.8×300, 0.8×30, per-claim basis ClaimCount over 10):
  OK. GE-office.txt (proceedings started on or after 2 June 2023) gives "Examination as to form and
  publication of application (up to 30 pages ...) 100", "Substantive examination (up to 10 claims)
  300" and "(for each additional claim over 10) 30". The old "Determination of subject-matter of
  protection" appears only in the pre-June-2023 table (l.440). The 20 % e-filing reduction covers
  all fees: GE-office-2.txt art. 6¹ "საფასურებზე გავრცელდეს 20%-იანი შეღავათი". GE.txt does not
  repeat the 20 % note on the per-claim line, but the resolution covers it.
- HR (3: 0.5×159.27, 0.5×0.66, 0.5×1.33): OK. HR.txt Annex: "Application filed in electronic format:
  50% of the applicable fee". HR-office-2.txt art. 6(2) "plaćaju se u iznosima umanjenim za 50%"
  covers items 1–3. (Rounding: 10 kn = 1.3272 EUR, so 0.5×1.33 overstates the fee by less than
  0.01 EUR per claim; negligible.)
- HU (4): OK. HU-office.txt s.5(2) "national fee is HUF 50 000, which is increased by HUF 2 500 ...
  HUF 5 000 ... HUF 7 500". s.5(3) gives exactly half (25 000 / 1 250 / 2 500 / 3 750), so the
  0.5 factor holds.
- IL (3): OK. IL.txt "Filing fee 2,402 ILS", "51st and each subsequent claim 616 ILS", "each
  additional 50 sheets that exceed 100 sheets 300 ILS".
- IQ (1): OK. IQ.txt l.178 national "Filing fee: 77,075 IQD". The baseline 77.075 was a thousands
  separator read as a decimal point, and the fix is correct.
- IR (2): OK. IR.txt l.195-198 national "Filing fee 2,500,000 IRR — in case of filing by a natural
  person 500,000 IRR". (The identical figures at l.132-135 are the RO transmittal fee; the editor
  quoted the national ones.)
- IS (3): OK. IS-office.txt "umsóknargjald 76.000", "viðbótargjald ... umfram tíu 4.900" and
  "Gjaldár 1./2./3. 13.000". The dots are thousands separators and were read correctly. In force
  15 February 2025 (l.291).
- IT (1: OFF_SheetFee removed): OK. IT.txt l.314-321 gives "Online 50 EUR" for every page band;
  the bands differ only on paper (120/160/400/600).
- JP (1: ExFee4 110000): OK. JP.txt l.411 "(d) ... designated under Japanese law: 110,000 JPY plus,
  per claim 3,200 JPY". The constant is still not yielded.
- KR (2): OK. KR.txt "Fee for request for examination 166,000 KRW ... For each claim plus 51,000
  KRW". KR-office.txt "166,000 / 51,000", "in effect since August 1, 2023".
- KZ (3): OK. KZ-office-2.txt, legal-entity column (spaces as thousands separators, commas as
  decimal points): "21 045,88", "69 350,60" and "for each independent clause in excess of one
  55 430,60". KZ.txt agrees.

## MATCH spot-checks (at least one per office)

All passed. Each value appears in the source and in the schedule:
AE 2,000 AED; AG 800 XCD; AL 7,000 ALL; AM 20,000 AMD; AO 27,722 AOA; AP 232 USD (online);
AT 52 EUR; AZ 20 AZN; BA 260 BAM; BB 300 BBD; BH 540 BHD; BN 1,100 BND; BW 240 BWP;
BZ 200/300 BZD renewals; CH CHF 200; CN 2,500 CNY; CR 525 USD; CU 11,040 CUP; CZ 1,200 CZK;
DE 350 EUR (no ISR); DJ 172,500 DJF; DO RD$13,225.00; DZ 5,000 DZD renewal; EA 40,000 RUB 2nd
independent claim; EC 596.49 USD; EE 225 EUR; EG 150 EGP; EP 135 EUR online filing, 102 EUR BA
extension; ES 593,65 € (IE04); GB 130 GBP electronic examination; GH 30,000 GHS; GM 5,000 GMD;
GT 2,500 GTQ; HN 1,500 HNL; HR 34.51 / 265.45 EUR; ID 75,000 IDR; IN 1,600 / 8,000 INR
electronic; IT 50 / 45 EUR online; JM 30,000 JMD; JO 200 JOD; JP 14,000 JPY; KE 250 USD;
KG 7,500 KGS; KH 320,000 KHR; KN 400 XCD; KP 230 EUR; KR 46,000 KRW electronic; KW 80 KWD;
KZ −20 % ISR reduction. BR, BY, CA, DK, FI, GE, HU, IL, IQ, IR and IS have no MATCH rows.

No figure in scope reads a thousands separator as a decimal point. None carries an amount in the
wrong currency, uses a paper rate where an electronic rate exists, or swaps a reduced and a full
rate.

## Discrepancies

All values below are supported by a source; in every case the record does not describe what the
schedule now says.

1. **BG**, PCT-BG.ipf l.7, 16, 102, 106, 110, 150; batch-01.md l.241, 246–248, 259.
   - Schedule: converted to EUR (`# Currency: EUR`, `YIELD 20.45+35.79`, `YIELD 92.03<EUR>`,
     `YIELD 10.23*PriorityCount`, `RETURN Currency AS 'EUR'`, `VERSION '2026.10' EFFECTIVE
     2026-01-01`).
   - Record: rows UNVERIFIED, "Version line: unchanged, '2024.1' EFFECTIVE 2024-01-01", "Per the
     protocol the schedule was not converted".
   - Source: BG-office-2.txt "1. Filing 20,45", "9. Publication ... 35,79", "7. Search and
     examination ... 92,03", "Claiming priority - for each priority 10,23 each"; BG.txt "EUR (Euro)".
   - The amounts are right. The currency switch also goes against PROTOCOL.md ("Keep the
     schedule's currency ... ask rather than converting") unless someone decided this after the
     record was written. The record and the summary table (BG 0 updated, 3 unverified) must be
     brought into line either way.
2. **BZ**, PCT-BZ.ipf l.167 (the former `YIELD 100<BZD>` is replaced by the comment "The first annuity
   is included in the 450 BZD filing fee."); batch-02.md l.117 and l.123–124.
   - Record: row "OFF_Renewal | 1st annuity | 100 | 100 | MATCH" and the note "The schedule still
     charges the 1st annuity (100) ... left unchanged".
   - Source: BZ-office.txt "On application for the grant of a Patent $450.00 (including 1st annuity
     fee)", which supports the removal.
   - Side effect: OFF_Renewal now yields nothing below 12 months (no explicit zero case; it has no
     VERIFY COMPLETE, so the checks are not affected).
3. **DE**, PCT-DE.ipf l.125: `YIELD RenFee IF RenMonth GTE 24`; batch-03.md l.36 and l.43.
   - Record: the row reads "OFF_Renewal3 | RenMonth ≥ 18 | ... MATCH" and the notes say "Condition
     doubt, left unchanged ... the schedule charges it from 18 months".
   - Source: DE.txt l.210 "due on the last day of the month containing the second anniversary (24
     months)". The change is right, but the record should say UPDATED (condition).
4. **DK**, PCT-DK.ipf l.118–119: the years 1 and 2 yields now read `IF RenMonth GTE 24` (baseline:
   always / `GTE 12`); batch-03.md l.69–70 and l.80.
   - Record: conditions "1st year (always)", "2nd year (RenMonth ≥ 12)", verdict UPDATED (amount)
     only, and the notes say "Condition doubt, left unchanged".
   - Source: DK.txt l.213 "These fees are due on the last day of the month containing the second
     anniversary (24 months)". The change is right, but it is not recorded.
5. **EP**, PCT-EP.ipf l.243: `YIELD RenFee IF RenMonth GTE 24`; batch-10.md l.35 and l.45.
   - Record: row "≥ 18 months from filing date"; the notes say "OFF_Renewal3 condition left
     unchanged at `RenMonth GTE 18`".
   - Source: EP.txt l.410 "due before the expiration of the month containing the second anniversary
     (24 months) of the international filing date". The change matches the source, but the editor
     had argued for keeping 18, so the record and the schedule now contradict each other.

Pattern: the same 18-to-24 renewal edit appears in DE, DK and EP (and the BZ and BG edits) without
a matching record entry. These edits were apparently made after the records were written (the
records date from 07:47–07:52, the diff from 08:28). Whoever made them should update the three
records.

Minor notes (not discrepancies): GE applies the 20 % e-filing reduction to the per-claim
examination fee, which GE-office-2.txt art. 6¹ supports although GE.txt omits it on that line.
HR's 0.5×1.33 has a rounding error below 0.01 EUR. AP's "from 101 ... 30" is read per page,
consistent with AP.txt "surcharge for each additional page from 101 30 USD".

## Totals

- Changed values checked: 112.
- Value supported by a saved source with the meaning the schedule gives it: 112.
- Fully confirmed (source and record both agree with the schedule): 103.
- Record does not agree with the schedule: 9 values in 5 offices (BG 5, BZ 1, DE 1, DK 1, EP 1).
