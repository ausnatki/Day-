// ============================================================
// Day 2：面向对象（OOP）—— 练习代码
// 运行方式：在当前目录执行  dotnet run
// 覆盖主题：
//   1. 封装 / 继承 / 多态
//   2. 抽象类 vs 接口
//   3. sealed（阻止继承/重写）
//   4. static（静态成员）
//   5. readonly vs const
//   6. 构造函数 / 静态构造 + new vs override
// ============================================================

// 让 Windows 终端正确显示中文
Console.OutputEncoding = System.Text.Encoding.UTF8;

Console.WriteLine("===== Day 2：面向对象（OOP）—— 练习代码 =====\n");

Demo1_EncapsulationInheritancePolymorphism();
Demo2_AbstractVsInterface();
Demo3_Sealed();
Demo4_Static();
Demo5_ReadonlyVsConst();
Demo6_ConstructorAndNewVsOverride();

Console.WriteLine("===== 全部完成 =====");


// ------------------------------------------------------------
// 【1】封装 / 继承 / 多态
// ------------------------------------------------------------
static void Demo1_EncapsulationInheritancePolymorphism()
{
    Console.WriteLine("【1】封装 / 继承 / 多态");

    // 封装：private 字段 + 公开属性，外部只能通过属性访问
    var stu = new Student { Name = "张三", Age = 20, School = "某大学" };
    Console.WriteLine($"  封装：{stu.Name}（{stu.Age} 岁，就读于 {stu.School}）");

    // 继承：Student 继承 Person，自动拥有 Name / Age / Introduce
    Console.WriteLine($"  继承：{stu.Introduce()}");

    // 多态：父类引用指向子类对象，调用 virtual/override 方法按实际类型执行
    Person p = stu;
    Console.WriteLine($"  多态：父类引用调用 Describe() → {p.Describe()}");

    Console.WriteLine();
}

// ------------------------------------------------------------
// 【2】抽象类 vs 接口
// ------------------------------------------------------------
static void Demo2_AbstractVsInterface()
{
    Console.WriteLine("【2】抽象类 vs 接口");

    // 抽象类：有抽象方法（无实现）+ 已实现的方法；不能 new
    // Animal animal = new Animal();   // 编译错误：抽象类不能实例化
    var bird = new Bird { Name = "小鸟" };
    Console.WriteLine($"  抽象类：{bird.Sound()}  // 抽象方法由子类实现");
    bird.Sleep();                                // 已实现的方法直接继承使用

    // 接口：只定义能力，一个类可以实现多个接口
    IFlyable flyable = bird;                     // 鸟实现了 IFlyable 接口
    flyable.Fly();

    Console.WriteLine("  结论：抽象类描述\"是什么\"（可复用实现、单继承），接口描述\"能做什么\"（只定义契约、可多实现）。");
    Console.WriteLine();
}

// ------------------------------------------------------------
// 【3】sealed
// ------------------------------------------------------------
static void Demo3_Sealed()
{
    Console.WriteLine("【3】sealed（阻止继承/重写）");

    var final = new SealedClass();
    final.Hi();

    // 下面两处被注释掉，因为会编译报错——取消注释看效果：
    // class ChildSealed : SealedClass { }         // 错误：不能从 sealed 类继承
    // class Grand : DerivedWithSealed { public override void M() { } }  // 错误：不能重写 sealed 方法

    var derived = new DerivedWithSealed();
    derived.M();                                    // sealed override 的方法仍可被调用

    Console.WriteLine("  结论：sealed 类不可被继承；sealed 方法不可被进一步重写（用于\"到此为止\"）。");
    Console.WriteLine();
}

// ------------------------------------------------------------
// 【4】static
// ------------------------------------------------------------
static void Demo4_Static()
{
    Console.WriteLine("【4】static（静态成员）");

    // 静态字段属于类型，所有实例共享同一份
    var c1 = new Counter();
    var c2 = new Counter();
    Console.WriteLine($"  c1.Id={c1.Id}, c2.Id={c2.Id}, 总创建数 Count={Counter.Count}   // 静态字段全局共享");

    // 静态类：不能 new，只能直接调静态方法
    Console.WriteLine($"  静态类 MathHelper.Add(3, 5) = {MathHelper.Add(3, 5)}");

    Console.WriteLine("  结论：静态成员属于类型本身，不用 new；静态类只能放静态成员。");
    Console.WriteLine();
}

// ------------------------------------------------------------
// 【5】readonly vs const
// ------------------------------------------------------------
static void Demo5_ReadonlyVsConst()
{
    Console.WriteLine("【5】readonly vs const");

    const double PI = 3.141592653589793;   // 编译期常量：声明时必须赋值，编译时就"写死"
    // PI = 3.14;                            // 编译错误：const 不能改

    var cfg = new Config(maxItems: 100);   // readonly：运行时（构造函数里）确定
    // cfg.MaxItems = 200;                  // 编译错误：readonly 构造后不能改

    Console.WriteLine($"  const PI = {PI}（编译期常量，隐式 static）");
    Console.WriteLine($"  readonly MaxItems = {cfg.MaxItems}（运行期构造时确定）");

    Console.WriteLine("  结论：const 编译期定死、只能是字面量；readonly 运行期在构造函数里赋值一次、类型不限。");
    Console.WriteLine();
}

// ------------------------------------------------------------
// 【6】构造函数 / 静态构造 + new vs override
// ------------------------------------------------------------
static void Demo6_ConstructorAndNewVsOverride()
{
    Console.WriteLine("【6】构造函数 / 静态构造 + new vs override");

    Console.WriteLine("  —— 构造顺序（首次使用 Pet 前，静态构造先执行一次）——");
    Pet dog = new Dog();                    // 触发：静态构造(一次) → 基类实例构造 → 子类实例构造
    dog.Speak();                            // override → 多态，按实际类型 Dog 执行

    Console.WriteLine("  —— new 隐藏 vs override 多态 ——");
    Pet cat = new Cat();                    // 声明类型是 Pet
    cat.Speak();                            // Pet.Speak（new 只是隐藏，父类引用看不到子类方法）
    Cat cat2 = new Cat();                   // 声明类型是 Cat
    cat2.Speak();                           // Cat.Speak（子类引用才能看到 new 隐藏的方法）

    Console.WriteLine("  结论：override=多态（按实际类型），new=隐藏（按声明类型）。构造顺序=静态构造→基类构造→子类构造。");
    Console.WriteLine();
}


// ============================================================
// 下面是各 demo 用到的类 / 接口（全部放底部，顶级语句必须在类型声明之前）
// ============================================================

// —— 演示封装/继承/多态的类 ——
class Person
{
    // 封装：字段私有，通过属性暴露
    private string _name = "";
    private int _age;

    public string Name { get => _name; set => _name = value; }
    public int Age
    {
        get => _age;
        set { if (value >= 0) _age = value; }   // 封装：在 setter 里做校验
    }

    public virtual string Describe() => "我是普通人";   // virtual：允许子类重写

    public string Introduce() => $"我叫 {Name}，{Age} 岁";
}

class Student : Person
{
    public string School { get; set; } = "";
    public override string Describe() => $"我是学生，就读于 {School}";   // override：重写（多态）
}

// —— 演示抽象类 vs 接口 ——
abstract class Animal
{
    public string Name { get; set; } = "";
    public abstract string Sound();                        // 抽象方法：没有实现，子类必须实现
    public void Sleep() => Console.WriteLine($"  {Name} 在睡觉（抽象类里已实现的方法）");
}

interface IFlyable
{
    void Fly();                                            // 接口方法：默认 public，只定义签名
}

class Bird : Animal, IFlyable
{
    public override string Sound() => "叽叽喳喳";
    public void Fly() => Console.WriteLine("  小鸟在飞（实现 IFlyable 接口）");
}

// —— 演示 sealed ——
sealed class SealedClass
{
    public void Hi() => Console.WriteLine("  sealed 类可以正常使用");
}

class BaseWithVirtual
{
    public virtual void M() => Console.WriteLine("  Base.M");
}

class DerivedWithSealed : BaseWithVirtual
{
    public sealed override void M() => Console.WriteLine("  Derived.M（sealed override：子类不能再重写）");
}

// —— 演示 static ——
class Counter
{
    public static int Count = 0;    // 静态字段：所有实例共享
    public int Id;
    public Counter() => Id = ++Count;
}

static class MathHelper
{
    public static int Add(int a, int b) => a + b;
}

// —— 演示 readonly vs const ——
class Config
{
    public readonly int MaxItems;          // readonly：只能在声明处或构造函数里赋值
    public Config(int maxItems) => MaxItems = maxItems;
}

// —— 演示 构造函数/静态构造 + new vs override ——
class Pet
{
    static Pet() => Console.WriteLine("  Pet 静态构造函数（类型首次使用前只执行一次）");
    public Pet() => Console.WriteLine("  Pet 实例构造函数");
    public virtual void Speak() => Console.WriteLine("  Pet.Speak（基类实现）");
}

class Dog : Pet
{
    public Dog() => Console.WriteLine("  Dog 实例构造函数");
    public override void Speak() => Console.WriteLine("  Dog.Speak（override 重写 → 多态）");
}

class Cat : Pet
{
    public Cat() => Console.WriteLine("  Cat 实例构造函数");
    public new void Speak() => Console.WriteLine("  Cat.Speak（new 隐藏，不是多态）");
}
