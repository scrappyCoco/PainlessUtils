[![.NET](https://github.com/scrappyCoco/PainlessUtils/actions/workflows/dotnet.yml/badge.svg?branch=main)](https://github.com/scrappyCoco/PainlessUtils/actions/workflows/dotnet.yml)
[![Nuget](https://img.shields.io/nuget/v/Coding4fun.PainlessUtils)](https://www.nuget.org/packages/Coding4fun.PainlessUtils/)

String extension that allows to change case to UPPER_CASE, camelCase, lowerCase, or something else.

```c#
const string sourceText = "SourceText";

string? upperCase   = sourceText.ChangeCase(CaseRules.ToUpperCase,       "_");
string? kebabCase   = sourceText.ChangeCase(CaseRules.ToLowerCase,       "-");
string? camelCase   = sourceText.ChangeCase(CaseRules.ToCamelCase,       "" );
string? capitalized = sourceText.ChangeCase(CaseRules.ToCapitalizedCase, " ");
string? titleCase   = sourceText.ChangeCase(CaseRules.ToTitleCase,       " ");

Assert.AreEqual("SOURCE_TEXT", upperCase);
Assert.AreEqual("source-text", kebabCase);
Assert.AreEqual("sourceText",  camelCase);
Assert.AreEqual("Source text", capitalized);
Assert.AreEqual("Source Text", titleCase);
```