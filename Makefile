ASS_SRCS=\
	MomaTool.Database/Issue.cs \
	MomaTool.Database/IssueType.cs \
	MomaTool.Database/MomaDefinition.cs \
	MomaTool.Database/Report.cs

ASS_ASSES=-r:Castle.ActiveRecord.dll -r:NHibernate.dll
ASS=MomaTool.Database.dll

TEST_SRCS=test.cs
TEST_ASSES=-r:$(ASS) $(ASS_ASSES)
TEST=test.exe
DBINS_SRCS=dbinsert.cs
DBINS_ASSES=-r:$(ASS) $(ASS_ASSES)
DBINS=dbinsert.exe

all: $(ASS) $(TEST) $(DBINS)

$(ASS): $(ASS_SRCS)
	gmcs -out:$(ASS) -target:library $(ASS_SRCS) $(ASS_ASSES)

$(TEST): $(TEST_SRCS) $(ASS)
	gmcs -out:$(TEST) $(TEST_SRCS) $(TEST_ASSES)

$(DBINS): $(DBINS_SRCS) $(ASS)
	gmcs -out:$(DBINS) $(DBINS_SRCS) $(DBINS_ASSES)
