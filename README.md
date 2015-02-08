# LatticeCodeGeneratorPad [![Build status](https://ci.appveyor.com/api/projects/status/v0p6wvn6fcosaaix?svg=true)](https://ci.appveyor.com/project/dotlattice/latticecodegeneratorpad)

A windows application that generates C# code from C# code.

## Code Generation

This project includes a class for generating C# code from an object tree.

For example, say we have this person class:

```c#
public class Person
{
    public string Name { get; set; }
}
```

We can use this to generate C# code for a very basic person:

```c#
var generator = new CSharpObjectCodeGenerator();
string code = generator.GenerateCode(new Person { Name = "Apollo" });
```

Which will give us basically what we passed in as a string:

```c#
@"new Person()
{
    Name = ""Apollo"",
}"
```

So you might be asking, what is the point of generating the same code that I passed in? You're right, what we did in that example was pretty stupid and pointless. But the key is that you can generate C# code from any object.

So for example, maybe we are reading these Person objects from a database or parsing them from some type of file. Here's an example that generates the same output, but uses an XML string as input instead:

```c#
var doc = XDocument.Parse(@"<Person><Name>Apollo</Name></Person>");
var person = new Person { Name = doc.Root.Element("Name").Value };
var code = new CSharpObjectCodeGenerator().GenerateCode(person);
```

One use for this type of code generating is creating unit tests. You can first call your real method that connects to a database or some other external resource, and then save the results as C# code. Then when you create a unit test, you can use that code as mock input to the class or method that you want to test.

The current version of the C# code generator is still fairly experimental. It works best on entities with a parameterless constructor and properties with public getters and setters.