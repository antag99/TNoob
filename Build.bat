:: Building TNoob for Windows
csc /target:winexe /define:NOOB /out:TNoob.exe TNoob.cs
csc /target:winexe /define:EXPERT /out:TExpert.exe TNoob.cs
PAUSE
