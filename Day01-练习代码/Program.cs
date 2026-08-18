// ============================================================
// Day 1：类型系统与内存 —— 练习代码
// 运行方式：在当前目录执行  dotnet run
// 覆盖主题：
//   1. 值类型 vs 引用类型
//   2. 装箱 / 拆箱
//   3. 装箱的性能损耗（Stopwatch 对比）
//   4. string 不可变性
//   5. StringBuilder vs 大量 += 拼接
//   6. 深拷贝 vs 浅拷贝
// ============================================================

using System.Diagnostics;

// 让 Windows 终端正确显示中文
Console.OutputEncoding = System.Text.Encoding.UTF8;

Console.WriteLine("===== Day 1：类型系统与内存 —— 练习代码 =====\n");

Demo1_ValueVsReference();
Demo2_BoxingUnboxing();
Demo3_BoxingPerformance();
Demo4_StringImmutability();
Demo5_StringBuilder();
Demo6_DeepShallowCopy();

Console.WriteLine("===== 全部完成 =====");


// ------------------------------------------------------------
// 【1】值类型 vs 引用类型
// ------------------------------------------------------------
static void Demo1_ValueVsReference()
{
    Console.WriteLine("【1】值类型 vs 引用类型");

    // 值类型：赋值时复制的是「值」，两份独立
    int a = 10;
    int b = a;      // b 是 a 的一份独立副本
    b = 99;
    Console.WriteLine($"  改 b 后：a={a}, b={b}   // 改 b 不影响 a（值类型复制的是值）");

    // 引用类型：赋值时复制的是「引用（地址）」，两个变量指向堆上同一个对象
    int[] arr1 = { 1, 2, 3 };
    int[] arr2 = arr1;   // arr2 和 arr1 指向同一个数组
    arr2[0] = 999;
    Console.WriteLine($"  改 arr2[0] 后：arr1[0]={arr1[0]}, arr2[0]={arr2[0]}   // 改 arr2 影响 arr1（复制的是引用）");

    // 默认值差异
    int defInt = default;        // 0
    string? defStr = default;    // null
    Console.WriteLine($"  默认值：int 默认={defInt}, string 默认={defStr ?? "null"}");

    Console.WriteLine();
}

// ------------------------------------------------------------
// 【2】装箱 / 拆箱
// ------------------------------------------------------------
static void Demo2_BoxingUnboxing()
{
    Console.WriteLine("【2】装箱 / 拆箱");

    int num = 42;
    object obj = num;        // 装箱（boxing）：隐式，把 int 包装成堆上的 object 对象
    int back = (int)obj;     // 拆箱（unboxing）：必须显式强转回「完全相同的原始类型」

    Console.WriteLine($"  num={num}, obj={obj}, back={back}");
    Console.WriteLine($"  类型：num={num.GetType()}, obj={obj.GetType()}");

    // 注意：拆箱强转成错误的类型会抛 InvalidCastException
    // 例如下面这行，因为装箱的是 int，不能拆箱成 long：
    //     long wrong = (long)obj;   // 运行时会抛异常
    Console.WriteLine("  提示：拆箱必须转回原始类型(int)，不能转成 long，否则抛 InvalidCastException");

    Console.WriteLine();
}

// ------------------------------------------------------------
// 【3】装箱的性能损耗
// ------------------------------------------------------------
static void Demo3_BoxingPerformance()
{
    Console.WriteLine("【3】装箱的性能损耗（Stopwatch 对比）");

    const int N = 10_000_000;
    var sw = new Stopwatch();

    // 方式一：泛型 List<int>，直接存 int，不装箱
    var list = new List<int>();
    sw.Restart();
    for (int i = 0; i < N; i++) list.Add(i);
    sw.Stop();
    Console.WriteLine($"  List<int>（无装箱）      : {sw.ElapsedMilliseconds} ms");

    // 方式二：ArrayList 的元素是 object，每次 Add 都要装箱
    var arrayList = new System.Collections.ArrayList();
    sw.Restart();
    for (int i = 0; i < N; i++) arrayList.Add(i);
    sw.Stop();
    Console.WriteLine($"  ArrayList（每次装箱）    : {sw.ElapsedMilliseconds} ms");

    Console.WriteLine("  结论：装箱 = 堆上分配对象 + 拷贝 + 增加 GC 压力，所以慢；泛型正是为消除装箱而设计。");
    Console.WriteLine();
}

// ------------------------------------------------------------
// 【4】string 不可变性
// ------------------------------------------------------------
static void Demo4_StringImmutability()
{
    Console.WriteLine("【4】string 为什么不可变");

    string s = "Hello";
    // s + " World" 不会修改原来的 s，而是创建一个全新的字符串
    string s2 = s + " World";
    Console.WriteLine($"  s=\"{s}\"（没变），s2=\"{s2}\"（新对象）");

    // 字符串驻留（interning）：相同的字面量只存一份
    string x = "abc";
    string y = "abc";
    Console.WriteLine($"  x=\"abc\", y=\"abc\"，是否同一对象 ReferenceEquals = {object.ReferenceEquals(x, y)}");

    Console.WriteLine("  好处：线程安全、可安全做字典 key、可安全共享；坏处：频繁拼接产生大量临时对象（见下一条）。");
    Console.WriteLine();
}

// ------------------------------------------------------------
// 【5】StringBuilder vs 大量 += 拼接
// ------------------------------------------------------------
static void Demo5_StringBuilder()
{
    Console.WriteLine("【5】StringBuilder vs 大量 += 拼接");

    const int N = 50_000;
    var sw = new Stopwatch();

    // 方式一：+= 每次生成新字符串，时间复杂度接近 O(n^2)
    string s = "";
    sw.Restart();
    for (int i = 0; i < N; i++) s += "a";
    sw.Stop();
    Console.WriteLine($"  string += 拼接 {N} 次 : {sw.ElapsedMilliseconds} ms");

    // 方式二：StringBuilder 内部维护可变缓冲区，O(n)
    var sb = new System.Text.StringBuilder();
    sw.Restart();
    for (int i = 0; i < N; i++) sb.Append("a");
    sw.Stop();
    Console.WriteLine($"  StringBuilder {N} 次  : {sw.ElapsedMilliseconds} ms");

    Console.WriteLine("  结论：循环里大量拼接用 StringBuilder；少量几次拼接直接用 + 即可（编译器会做优化）。");
    Console.WriteLine();
}

// ------------------------------------------------------------
// 【6】深拷贝 vs 浅拷贝
// ------------------------------------------------------------
static void Demo6_DeepShallowCopy()
{
    Console.WriteLine("【6】深拷贝 vs 浅拷贝");

    var p1 = new Person { Name = "张三", Scores = new[] { 90, 80 } };

    // 浅拷贝：只复制最外层，引用类型字段 Scores 仍指向同一个数组
    var shallow = p1.ShallowCopy();
    shallow.Scores[0] = 0;
    Console.WriteLine($"  浅拷贝：p1.Scores[0]={p1.Scores[0]}, shallow.Scores[0]={shallow.Scores[0]}   // 共享同一数组（被影响）");

    // 深拷贝：引用类型字段也复制了一份，完全独立
    var deep = p1.DeepCopy();
    deep.Scores[0] = -1;
    Console.WriteLine($"  深拷贝：p1.Scores[0]={p1.Scores[0]}, deep.Scores[0]={deep.Scores[0]}   // 各自独立（互不影响）");

    Console.WriteLine("  结论：浅拷贝复制引用，深拷贝复制内容；含引用字段时需手动深拷贝。");
    Console.WriteLine();
}

// 用于演示深浅拷贝的类
class Person
{
    public string Name = "";               // 引用类型字段
    public int[] Scores = Array.Empty<int>(); // 引用类型字段

    // 浅拷贝：MemberwiseClone 只复制最外层，引用字段仍指向原对象
    public Person ShallowCopy() => (Person)MemberwiseClone();

    // 深拷贝：new 一个对象，把引用字段也 Clone 一份
    public Person DeepCopy() => new Person
    {
        Name = this.Name,
        Scores = (int[])this.Scores.Clone()
    };
}
