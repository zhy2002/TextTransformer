Overview
==========

RTScript is a simple scripting language I created based on Lisp and JavaScript. It can be embedded in plain text and generate dynamic content in place. In another word it is a template engine.
There are two places in TextTransformer you can use RTScript:


1. In the "replace" text area: in this context you can use the items in a match
2. In the "merge" text area: in this context you can use everything available in the merge stage.  

Below is a description of the script language.


to be provided...

1. Literal and Expression List
An RT template converts an RTMatch object into a string. It is a block of script executed by the RT interpreter.
There are 2 lexical contexts in RT template, namely Literal and Expression List.
By default we are in Literal context, and everything will be sent to the output as is. E.g. the following script:
This is a literal
The only exception is for "@" symbol. Two consecutive "@" symbols are output as one, so that output literal "@{" is possible (use "@@{").
is interpreted as:
This is a literal
You can switch from Literal context to Script context by context switch mark:
... @{ ... @} ...
In an expression list, you can switch back to Literal context by using context switch mark again:
..literal.. @{ ..expression list.. @{ ..literal.. @} ..expression list.. @} ..literal..
In an expression, a literal block serves as a string.

2. Data types
There are 3 datatypes, bool, number and string. All these three types are nullable, which means the equivalent .Net types are bool?, double? and string.
Since the RT template is used to generate text from regular expression matching result, all source values are string literal.
Therefore the type conversion is enforced by the functions at parameter level. See RTConverter class for how values are converted between these three native data types.

3. Program Structure
RT script is composed of functions. A piece of RT script is a hierachy of function calls, where all functions are eagarly evaluated. E.g.
(* 2 (+ 4 5))
The result will be 18. 
In Script context, you can type in a list of top level functions, the evaluation result of these functions will be inserted into the output stream.
Each function can evaluate to a concrete value or a partially applied function, which is represented by a scope object. 
The execution of each function will create a scope object, where nested function calls executes (can get or set outer variables).
If the function has all parameters filled, the function will evaluate to a specific value in the end. Otherwise a partial function is returned.
E.g. (* 3 x)
Evaluates to a partial function x=>3*X

 

4. 


//todo
 - RTScope to string -> script text for the scope
 



         /// <summary>
        /// @{ (Reverse (def a ($0)) (a)) @}
        /// @{ (For (def body (Lower x)) 1 10 (body)) @}
        /// </summary>
        Functional,

        /// <summary>
        /// {funname1 arg1 arg2 {funcname2 arg1}}
        /// {0}
        /// {ToLower {0}}
        /// {Substring {0} 0 {Add {Len {0}} -1}}
        /// {IndexOf {0} "Test"}
        /// </summary>
        Simple

