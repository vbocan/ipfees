DEFINE LIST EntityType AS 'Select the desired entity type'
VALUE 'Normal' AS NormalEntity
VALUE 'Small' AS SmallEntity
VALUE 'Micro' AS MicroEntity
DEFAULT NormalEntity
ENDDEFINE

DEFINE LIST SituationType AS 'Situation type'
VALUE 'IRRP (Chapter II) prepared by the IPEA/US' AS PreparedIPEA
VALUE 'International search fee paid as ISA' AS PaidAsISA
VALUE 'Search report prepared by an ISA other than the US' AS PreparedISA
VALUE 'All other situations' AS AllOtherSituations
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
YIELD 320 IF EntityType IS NormalEntity
YIELD 128 IF EntityType IS SmallEntity 
YIELD 64 IF EntityType IS MicroEntity
ENDCOMPUTE

COMPUTE FEE SearchFee
CASE SituationType IS PreparedIPEA AS
	YIELD 0 IF EntityType IS NormalEntity
	YIELD 0 IF EntityType IS SmallEntity
	YIELD 0 IF EntityType IS MicroEntity
ENDCASE
CASE SituationType IS PaidAsISA AS
	YIELD 140 IF EntityType IS NormalEntity
	YIELD 56 IF EntityType IS SmallEntity
	YIELD 28 IF EntityType IS MicroEntity
ENDCASE
CASE SituationType IS PreparedISA AS
	YIELD 540 IF EntityType IS NormalEntity
	YIELD 216 IF EntityType IS SmallEntity
	YIELD 108 IF EntityType IS MicroEntity
ENDCASE
YIELD 700 IF EntityType IS NormalEntity
YIELD 280 IF EntityType IS SmallEntity
YIELD 140 IF EntityType IS MicroEntity
ENDCOMPUTE

COMPUTE FEE ExaminationFee
CASE SituationType IS PreparedIPEA AS
	YIELD 0 IF EntityType IS NormalEntity
	YIELD 0 IF EntityType IS SmallEntity
	YIELD 0 IF EntityType IS MicroEntity
ENDCASE
YIELD 800 IF EntityType IS NormalEntity
YIELD 360 IF EntityType IS SmallEntity
YIELD 160 IF EntityType IS MicroEntity
ENDCOMPUTE

COMPUTE FEE SheetFee
YIELD 420 * SheetCount / 50 IF SheetCount ABOVE 100 AND EntityType IS NormalEntity
YIELD 168 * SheetCount / 50 IF SheetCount ABOVE 100 AND EntityType IS SmallEntity
YIELD 84 * SheetCount / 50 IF SheetCount ABOVE 100 AND EntityType IS MicroEntity
ENDCOMPUTE

COMPUTE FEE ClaimFee
YIELD 480 * ClaimCount IF ClaimCount ABOVE 3 AND EntityType IS NormalEntity
YIELD 192 IF ClaimCount ABOVE 3 AND EntityType IS SmallEntity
YIELD 96 IF ClaimCount ABOVE 3 AND EntityType IS MicroEntity
ENDCOMPUTE