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

#### Identifier
*Example:* `foo` or `foo()`
```
identifierexpr
	::= identifier
    ::= identifier '(' expression ')'
```

#### Variable declaration expression (WIP)
*Example:* `let foo = 3` or `let foo = 1 + 2` or `let foo = bar + 1`
```
declarationexpr
    ::= 'let' identifier
    ::= 'let' identifier '=' expression
```

#### Primary expression
*Example:* `foo` or `3` or `(1 + 2)`
```
primaryexpr
	::= identifierexpr
	::= numberexpr
	::=	parenexpr
```

#### Binary operator right hand side
*Example:* `+ 3` or `* 2`
```
binoprhs
	::= ('+' primary)*
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
