# zcl.calculator
This project implements a simple yet extensible calculator in C#. All values are stored as double.

###Features
* Can store result in a variable
* Support unary and binary operators
* Can define your own operator, function and constant by subclassing <code>CommandConfigurer</code>
* By default support constants <code>PI</code>, <code>E</code>; operators: <code>+</code>, <code>-</code>, <code>*</code>, <code>/</code>, <code>%</code>, <code>^</code>, <code>!</code> (<small>negate when used as prefix, factorial used as suffix</small>), <code>=</code> (<small>equal comparison</small>), <code>!=</code>, <code>&gt;</code>, <code>&gt;=</code>, <code>&lt;</code>, <code>&lt;=</code>; functions <code>log</code>, <code>sin</code>, <code>cos</code>, <code>tan</code>, <code>sum</code>, <code>nan</code>.

###Example
<pre>
  ICalculator calculator = new CalculatorFactory().createInstance();
  double retval = calc.Compute("(5*2)!+3 = 3628803");
  Assert.AreEqual(1d, retval);
</pre>

<pre>
  ....
  retval = calc.Compute("x1:= 12.2");
  retval = calc.Compute("y1:= 98.23");
  retval = calc.Compute("x2:= -43.21");
  retval = calc.Compute("y2:= 71.4302");
  retval = calc.Compute("((x2-x1)^2+(y2-y1)^2)^0.5");
</pre>


More details see the unit tests.