# =============================================================================
# IPFLang Jurisdiction: PCT-US
# =============================================================================
# Entry of national phase in the United States of America
#
# Category: OfficialFees
# Currency: USD
# Phase: National Phase
#
# Usage:
#   dotnet run --project ../src/IPFLang.CLI -- run ./PCT-US.ipf
#
# =============================================================================

VERSION '2026.10' EFFECTIVE 2025-01-19 DESCRIPTION 'Entry of national phase in the United States of America'


# Groups
DEFINE GROUP GeneralGroup AS 'General Settings' WITH WEIGHT 1
DEFINE GROUP ApplicationGroup AS 'Application Details' WITH WEIGHT 2
DEFINE GROUP PCTGroup AS 'PCT Information' WITH WEIGHT 3

# Entity size (includes all entity classifications used globally)
DEFINE LIST EntitySize AS 'Entity size of applicant'
GROUP GeneralGroup
CHOICE Entity_Person AS 'Natural person'
CHOICE Entity_Less25 AS 'Less than 25 employees'
CHOICE Entity_Less100 AS 'Less than 100 employees'
CHOICE Entity_Small AS 'Small entity'
CHOICE Entity_Micro AS 'Micro entity'
CHOICE Entity_Large AS 'Large entity (default)'
DEFAULT Entity_Large
ENDDEFINE

# Application date
DEFINE DATE ApplicationDate AS 'Application date (international filing date)'
GROUP ApplicationGroup
BETWEEN 01.01.2000 AND TODAY
DEFAULT TODAY
ENDDEFINE

# Claims
DEFINE NUMBER ClaimCount AS 'Number of claims'
GROUP ApplicationGroup
BETWEEN 1 AND 1000
DEFAULT 10
ENDDEFINE

# Sheets/Pages
DEFINE NUMBER SheetCount AS 'Number of pages/sheets'
GROUP ApplicationGroup
BETWEEN 1 AND 10000
DEFAULT 30
ENDDEFINE

# Priority count
DEFINE NUMBER PriorityCount AS 'Number of priorities claimed'
GROUP ApplicationGroup
BETWEEN 0 AND 100
DEFAULT 1
ENDDEFINE

# Independent claim count
DEFINE NUMBER IndependentClaimCount AS 'Number of independent claims'
GROUP ApplicationGroup
BETWEEN 1 AND 100
DEFAULT 3
ENDDEFINE

# Multiple dependent claim count
DEFINE NUMBER MultipleDependentClaimCount AS 'Number of multiple dependent claims'
GROUP ApplicationGroup
BETWEEN 0 AND 100
DEFAULT 0
ENDDEFINE

# Dependent claim count (for jurisdictions that charge per dependent claim)
DEFINE NUMBER DependentClaimCount AS 'Number of dependent claims'
GROUP ApplicationGroup
BETWEEN 0 AND 1000
DEFAULT 7
ENDDEFINE

# Country count (for regional organizations like ARIPO)
DEFINE NUMBER CountryCount AS 'Number of designated countries'
GROUP ApplicationGroup
BETWEEN 1 AND 50
DEFAULT 1
ENDDEFINE

# PCT-specific inputs
DEFINE LIST ISA AS 'Which ISA searched the PCT application?'
GROUP PCTGroup
CHOICE ISA_NONE AS 'None of the listed'
CHOICE ISA_EPO AS 'EP - European Patent Office'
CHOICE ISA_US AS 'US - USPTO'
CHOICE ISA_JP AS 'JP - Japan Patent Office'
CHOICE ISA_KR AS 'KR - Korean IP Office'
CHOICE ISA_CN AS 'CN - CNIPA'
CHOICE ISA_AT AS 'AT - Austrian Patent Office'
CHOICE ISA_AU AS 'AU - IP Australia'
CHOICE ISA_CA AS 'CA - Canadian IP Office'
CHOICE ISA_ES AS 'ES - Spanish Patent Office'
CHOICE ISA_FI AS 'FI - Finnish Patent Office'
CHOICE ISA_RU AS 'RU - Rospatent'
CHOICE ISA_SE AS 'SE - Swedish Patent Office'
CHOICE ISA_TR AS 'TR - Turkish Patent Office'
CHOICE ISA_NPI AS 'XN - Nordic Patent Institute'
CHOICE ISA_VI AS 'XV - Visegrad Patent Institute'
DEFAULT ISA_NONE
ENDDEFINE

DEFINE LIST IPRP AS 'Which office provided an IPRP (Chapter II)?'
GROUP PCTGroup
CHOICE IPRP_NONE AS 'No IPRP / Chapter I only'
CHOICE IPRP_EPO AS 'EP - European Patent Office'
CHOICE IPRP_US AS 'US - USPTO'
CHOICE IPRP_JP AS 'JP - Japan Patent Office'
CHOICE IPRP_KR AS 'KR - Korean IP Office'
CHOICE IPRP_CN AS 'CN - CNIPA'
CHOICE IPRP_AT AS 'AT - Austrian Patent Office'
CHOICE IPRP_AU AS 'AU - IP Australia'
CHOICE IPRP_RU AS 'RU - Rospatent'
DEFAULT IPRP_NONE
ENDDEFINE

DEFINE BOOLEAN ISR_AVAILABLE AS 'Is International Search Report available?'
GROUP PCTGroup
DEFAULT TRUE
ENDDEFINE

DEFINE BOOLEAN RO_NP AS 'Is receiving office same as national phase office?'
GROUP PCTGroup
DEFAULT FALSE
ENDDEFINE

DEFINE BOOLEAN OfficeElected AS 'Is the designated office an elected office?'
GROUP PCTGroup
DEFAULT FALSE
ENDDEFINE

DEFINE BOOLEAN Extension AS 'Request extension to additional states?'
GROUP PCTGroup
DEFAULT FALSE
ENDDEFINE

# =============================================================================
# Official Fees
# =============================================================================

# File name: PCT-US-OFF
# File content: Official fees for PCT national phase: United States of America
# Checked against the fees in force on 20 October 2026 (USPTO fee schedule eff. 19 Jan 2025, eGuide US)
# Is newer than 30 days: False
# Fees keywords: ['claims', 'claim']

COMPUTE FEE OFF_BasicNationalFee
LET Fee1 AS 350
LET Fee2 AS 140
LET Fee3 AS 70

YIELD Fee1 IF EntitySize NEQ Entity_Small AND EntitySize NEQ Entity_Micro
YIELD Fee2 IF EntitySize EQ Entity_Small
YIELD Fee3 IF EntitySize EQ Entity_Micro
ENDCOMPUTE

COMPUTE FEE OFF_SearchFee

CASE IPRP EQ IPRP_US AS
YIELD 0
ENDCASE

CASE ISA EQ ISA_US AND ISR_AVAILABLE EQ TRUE AND IPRP NEQ IPRP_US AS
YIELD 150<USD> IF EntitySize NEQ Entity_Small AND EntitySize NEQ Entity_Micro
YIELD 60<USD> IF EntitySize EQ Entity_Small
YIELD 30<USD> IF EntitySize EQ Entity_Micro
ENDCASE

CASE ISR_AVAILABLE EQ TRUE AND ISA NEQ ISA_US AND IPRP NEQ IPRP_US AS
YIELD 580<USD> IF EntitySize NEQ Entity_Small AND EntitySize NEQ Entity_Micro
YIELD 232<USD> IF EntitySize EQ Entity_Small
YIELD 116<USD> IF EntitySize EQ Entity_Micro
ENDCASE

CASE ISR_AVAILABLE EQ FALSE AND IPRP NEQ IPRP_US AS
YIELD 770<USD> IF EntitySize NEQ Entity_Small AND EntitySize NEQ Entity_Micro
YIELD 308<USD> IF EntitySize EQ Entity_Small
YIELD 154<USD> IF EntitySize EQ Entity_Micro
ENDCASE

ENDCOMPUTE

COMPUTE FEE OFF_ExaminationFee

CASE IPRP EQ IPRP_US AS
YIELD 0
ENDCASE

CASE IPRP NEQ IPRP_US AS
YIELD 880<USD> IF EntitySize NEQ Entity_Small AND EntitySize NEQ Entity_Micro
YIELD 352<USD> IF EntitySize EQ Entity_Small
YIELD 176<USD> IF EntitySize EQ Entity_Micro
ENDCASE

ENDCOMPUTE

COMPUTE FEE OFF_SheetFee
YIELD 450*CEIL( ( SheetCount - 100 ) / 50 ) IF SheetCount GT 100 AND EntitySize NEQ Entity_Small AND EntitySize NEQ Entity_Micro
YIELD 180*CEIL( ( SheetCount - 100 ) / 50 ) IF SheetCount GT 100 AND EntitySize EQ Entity_Small
YIELD 90*CEIL( ( SheetCount - 100 ) / 50 ) IF SheetCount GT 100 AND EntitySize EQ Entity_Micro
# Nothing is due in the remaining case; stated so the schedule answers it.
YIELD 0 IF SheetCount LTE 100
ENDCOMPUTE

COMPUTE FEE OFF_IndependentClaimFee
YIELD 600*(IndependentClaimCount-3) IF IndependentClaimCount GT 3 AND EntitySize NEQ Entity_Small AND EntitySize NEQ Entity_Micro
YIELD 240*(IndependentClaimCount-3) IF IndependentClaimCount GT 3 AND EntitySize EQ Entity_Small
YIELD 120*(IndependentClaimCount-3) IF IndependentClaimCount GT 3 AND EntitySize EQ Entity_Micro
# Nothing is due in the remaining case; stated so the schedule answers it.
YIELD 0 IF IndependentClaimCount LTE 3
ENDCOMPUTE

COMPUTE FEE OFF_ClaimFee
YIELD 200*(ClaimCount-20) IF ClaimCount GT 20 AND EntitySize NEQ Entity_Small AND EntitySize NEQ Entity_Micro
YIELD 80*(ClaimCount-20) IF ClaimCount GT 20 AND EntitySize EQ Entity_Small
YIELD 40*(ClaimCount-20) IF ClaimCount GT 20 AND EntitySize EQ Entity_Micro
# Nothing is due in the remaining case; stated so the schedule answers it.
YIELD 0 IF ClaimCount LTE 20
ENDCOMPUTE

COMPUTE FEE OFF_MultipleDependentClaimFee
# Charged once for an application that contains a multiple dependent claim, not per claim.
YIELD 925 IF MultipleDependentClaimCount GT 0 AND EntitySize NEQ Entity_Small AND EntitySize NEQ Entity_Micro
YIELD 370 IF MultipleDependentClaimCount GT 0 AND EntitySize EQ Entity_Small
YIELD 185 IF MultipleDependentClaimCount GT 0 AND EntitySize EQ Entity_Micro
# Nothing is due in the remaining case; stated so the schedule answers it.
YIELD 0 IF MultipleDependentClaimCount EQ 0
ENDCOMPUTE

#annuities after grant

# =============================================================================
# Translation Fees (OPTIONAL)
# Language: English
# =============================================================================

DEFINE NUMBER NumberOfWords AS 'Number of words to translate'
GROUP ApplicationGroup
BETWEEN 1 AND 10000000
DEFAULT 5000
ENDDEFINE

COMPUTE FEE TranslationFee OPTIONAL
# Translation fee per word into English
# Translation fee into local language per 100 words; source language is English
YIELD 12<EUR> * NumberOfWords / 100
ENDCOMPUTE

# =============================================================================
# Service Fees (OPTIONAL)
# =============================================================================

COMPUTE FEE AgentServiceFee OPTIONAL
YIELD 100<EUR>
ENDCOMPUTE

# =============================================================================
# Verification Directives
# =============================================================================

VERIFY COMPLETE FEE OFF_BasicNationalFee
VERIFY COMPLETE FEE OFF_SearchFee
VERIFY COMPLETE FEE OFF_ExaminationFee
VERIFY COMPLETE FEE OFF_SheetFee
VERIFY COMPLETE FEE OFF_IndependentClaimFee
VERIFY COMPLETE FEE OFF_ClaimFee
VERIFY COMPLETE FEE OFF_MultipleDependentClaimFee
VERIFY MONOTONIC FEE OFF_SheetFee WITH RESPECT TO SheetCount
VERIFY MONOTONIC FEE OFF_IndependentClaimFee WITH RESPECT TO IndependentClaimCount
VERIFY MONOTONIC FEE OFF_ClaimFee WITH RESPECT TO ClaimCount
VERIFY MONOTONIC FEE OFF_MultipleDependentClaimFee WITH RESPECT TO MultipleDependentClaimCount

# =============================================================================
# Returns
# =============================================================================

RETURN Currency AS 'USD'
RETURN Jurisdiction AS 'US'
RETURN JurisdictionName AS 'Entry of national phase in the United States of America'
