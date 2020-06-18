# Grammar

#### Numerical literal
*Example:* `1`
```
numberexpr ::= number
```

#### Parenthesis
*Example:* `(1 + 2)`
```
parenexpr ::= '(' expression ')'
```

#### Identifier expression
*Example:* `foo` or `foo()` or `foo = 1` or `foo = bar + 1`
```
identifierexpr
	::= identifier
    ::= identifier '(' expression* ')'
	::= identifier '=' expression
```

#### Variable declaration statement
*Example:* `let foo = 3` or `let foo = 1 + 2` or `let foo = bar + 1`
```
declarationstatement
    ::= 'let' identifier
    ::= 'let' identifier '=' expression
```

#### Function declaration (WIP)
*Example:* `function foo() => 1 + 1` or `function foo(x) => x * x` or `function foo(x, y) => x + y`
```
functiondeclaration
	::= 'function' identifier '(' expression* ')' '=>' expression
```

#### Primary expression
*Example:* `foo` or `3` or `(1 + 2)`
```
primaryexpr
	::= numberexpr
	::= parenexpr
	::= identifierexpr
```

#### Binary operator right hand side
*Example:* `+ 3` or `* 2`
```
binoprhs
	::= (operator primary)*
```

#### Expression
*Example:* `1 + 2` or `bar + 1` or `(1 + 2) * 3`
```
expression
    ::= primary binoprhs
```

#### Top level expression
```
toplevelexpr ::= expression
```

#### Statement
```
statement
	::= toplevelexpr ';'
	::= declarationstatement
```
