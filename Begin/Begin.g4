grammar Begin;
options
{
	language=CSharp3;
}

tokens
{
	NEGATE;
	STRING;
	CONCAT;
}

code
	:	block+ EOF
	;

block
	:	functionDecl	#FuncDecl
	|	statement	#Stmnt
	;

statement
	:	functionCall ';'	#FuncCall
	|	ifFunctionCall		#IfFuncCall
	|	variableDecl ';'	#VarDecl
	|	assignment ';'		#Assg
	;

variableDecl
	:	DATATYPE CONTEXTVALUE ('=' logicalExpression)?
	;

assignment
	:	CONTEXTVALUE '=' logicalExpression	#AssgVar
	|	CONTEXTVALUE INCREMENT				#IncVar
	|	CONTEXTVALUE DECREMENT				#DecVar
	;
		
logicalExpression //Should be a way to write this rule with logicalOr and logicalAnd rule in it to replace those next 2 rules, but this works for the time being
	:	logicalOr
	;
	
logicalOr
	:	logicalOr OR logicalOr	#LogOr
	|	logicalAnd				#LogAndEx
	;
	
logicalAnd
	:	logicalAnd AND logicalAnd	#LogAnd
	|	equalityExpression			#EqEx
	;

equalityExpression
	:	equalityExpression op=(EQUALS|NOTEQUALS) equalityExpression	#EqNoteq
	|	relationalExpression										#RelEx
	;
	
relationalExpression
	:	relationalExpression op=(LT|LTEQ|GT|GTEQ) relationalExpression	#LTGT
	|	numberExpression												#NumEx
	;

numberExpression 
	:	numberExpression op=(MULT|DIV|MOD) numberExpression	#MulDivMod
	|	numberExpression op=(PLUS|MINUS) numberExpression	#PlMin
	|	unaryExpression										#PriEx
	;

unaryExpression
	:	primaryExpression					#PriExp
    |	op=(NOT|MINUS) primaryExpression	#NegEx
	|	INTEGER INCREMENT					#IncEx
	|	INTEGER DECREMENT					#DecEx
   	;

primaryExpression
	:	'(' logicalExpression ')'	#Parens
	|	value						#Val
	;

value	
	: 	INTEGER		#Int
	|	FLOAT		#Float
	|	BOOLEAN		#Bool
	|	STRING		#Str
	|	DATATYPE CONTEXTVALUE #VarDec
	|	CONTEXTVALUE #ContextVal
	;

AND		: 	'&&' | 'and';
OR 		: 	'||' | 'or';

EQUALS	:	'=';
NOTEQUALS :	'!=';

LT		:	'<';
LTEQ	:	'<=';
GT		:	'>';
GTEQ	:	'>=';

PLUS	:	'+';
MINUS	:	'-';
	
MULT	:	'*';
DIV		:	'/';
MOD		:	'%';
  
NOT		:	'!' | 'not';
INCREMENT : '++';
DECREMENT : '--';
	
STRING	:  	'"' ( EscapeSequence | ~('\u0000'..'\u001f' | '\\' | '"' ) )* '"';

INTEGER	:	('0'..'9')+;

FLOAT	:	('0'..'9')* '.' ('0'..'9')+;

BOOLEAN	:	'true'
		|	'false'
		;

fragment Identifier	: IdentifierFirst (IdentifierRest)*;
fragment IdentifierFirst: 'a'..'z' | 'A'..'Z' | '_';
fragment IdentifierRest : 'a'..'z' | 'A'..'Z' | '_' | '0'..'9';

// Must be declared after the BOOLEAN token or it will hide it
functionDecl
	:	'func' CONTEXTVALUE '(' ( DATATYPE CONTEXTVALUE (',' DATATYPE CONTEXTVALUE)* )? ')' functionBody
	;

functionCall
	:	CONTEXTVALUE '(' ( (logicalExpression|assignment) (',' (logicalExpression|assignment))* )? ')'
	;

ifFunctionCall
	:	'if' '(' logicalExpression ')' functionBody
	;

functionBody
	:	'{' statement+ '}'
	;
	
DATATYPE
	:	'int'
	|	'decimal'
	|	'bool'
	|	'string'
	;

CONTEXTVALUE
	:	Identifier// ('.' Identifier)*
	;

fragment EscapeSequence 
	:	'\\'('n' | 'r' | 't' | '\'' | '\\' | UnicodeEscape)
	;
fragment UnicodeEscape
    :    	'u' HexDigit HexDigit HexDigit HexDigit 
    ;
fragment HexDigit 
	: 	('0'..'9'|'a'..'f'|'A'..'F') 
	;

WS
	:	(' '|'\r'|'\t'|'\u000C'|'\n') -> channel(HIDDEN)
	;
