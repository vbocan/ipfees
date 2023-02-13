DEFINE LIST EntityType AS 'Select the desired entity type'
CHOICE 'Normal' AS NormalEntity
CHOICE 'Small' AS SmallEntity
CHOICE 'Micro' AS MicroEntity
DEFAULT NormalEntity
ENDDEFINE

DEFINE LIST SituationType AS 'Situation type'
CHOICE 'IRRP (Chapter II) prepared by the IPEA/US' AS PreparedIPEA
CHOICE 'International search fee paid as ISA' AS PaidAsISA
CHOICE 'Search report prepared by an ISA other than the US' AS PreparedISA
CHOICE 'All other situations' AS AllOtherSituations
DEFAULT PreparedIPEA
ENDDEFINE

DEFINE NUMBER SheetCount AS 'Enter the number of sheets'
BETWEEN 10 AND 1000
DEFAULT 15
ENDDEFINE

DEFINE NUMBER ClaimCount AS 'Number of claims'
BETWEEN 0 AND 1000
DEFAULT 0
ENDDEFINE

DEFINE BOOLEAN ContainsDependentClaims AS 'Does this contain dependent claims?'
DEFAULT TRUE
ENDDEFINE

COMPUTE FEE BasicNationalFee
YIELD 320 IF EntityType EQUALS NormalEntity
YIELD 128 IF EntityType EQUALS SmallEntity 
YIELD 64 IF EntityType EQUALS MicroEntity
ENDCOMPUTE

COMPUTE FEE SearchFee
CASE SituationType EQUALS PreparedIPEA AS
	YIELD 0 IF EntityType EQUALS NormalEntity
	YIELD 0 IF EntityType EQUALS SmallEntity
	YIELD 0 IF EntityType EQUALS MicroEntity
ENDCASE
CASE SituationType EQUALS PaidAsISA AS
	YIELD 140 IF EntityType EQUALS NormalEntity
	YIELD 56 IF EntityType EQUALS SmallEntity
	YIELD 28 IF EntityType EQUALS MicroEntity
ENDCASE
CASE SituationType EQUALS PreparedISA AS
	YIELD 540 IF EntityType EQUALS NormalEntity
	YIELD 216 IF EntityType EQUALS SmallEntity
	YIELD 108 IF EntityType EQUALS MicroEntity
ENDCASE
YIELD 700 IF EntityType EQUALS NormalEntity
YIELD 280 IF EntityType EQUALS SmallEntity
YIELD 140 IF EntityType EQUALS MicroEntity
ENDCOMPUTE

COMPUTE FEE ExaminationFee
CASE SituationType EQUALS PreparedIPEA AS
	YIELD 0 IF EntityType EQUALS NormalEntity
	YIELD 0 IF EntityType EQUALS SmallEntity
	YIELD 0 IF EntityType EQUALS MicroEntity
ENDCASE
YIELD 800 IF EntityType EQUALS NormalEntity
YIELD 360 IF EntityType EQUALS SmallEntity
YIELD 160 IF EntityType EQUALS MicroEntity
ENDCOMPUTE

COMPUTE FEE SheetFee
YIELD 420*((SheetCount-100)/50) IF SheetCount ABOVE 100 AND EntityType EQUALS NormalEntity
YIELD 168*((SheetCount-100)/50) IF SheetCount ABOVE 100 AND EntityType EQUALS SmallEntity
YIELD 84*((SheetCount-100)/50) IF SheetCount ABOVE 100 AND EntityType EQUALS MicroEntity
ENDCOMPUTE

COMPUTE FEE ClaimFee
YIELD (480*ClaimCount) IF ClaimCount ABOVE 3 AND EntityType EQUALS NormalEntity
YIELD (192*ClaimCount) IF ClaimCount ABOVE 3 AND EntityType EQUALS SmallEntity
YIELD (96*ClaimCount) IF ClaimCount ABOVE 3 AND EntityType EQUALS MicroEntity
ENDCOMPUTE