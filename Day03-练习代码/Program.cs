// ============================================================
// Day 3：集合与泛型 —— 练习代码
// 运行方式：在当前目录执行  dotnet run
// 覆盖主题：
//   1. List<T> vs 数组
//   2. Dictionary<K,V>（哈希表底层）
//   3. HashSet / Queue / Stack
//   4. 泛型约束（where T : ...）
//   5. 协变 out / 逆变 in
//   6. IEnumerable vs ICollection vs IList
// ============================================================

// 让 Windows 终端正确显示中文
using System;
using System.Text.Json;
using System.Text.Json.Serialization;

Console.OutputEncoding = System.Text.Encoding.UTF8;

Console.WriteLine("===== Day 3：集合与泛型 —— 练习代码 =====\n");

Demo1_ListVsArray();
Demo2_Dictionary();
Demo3_HashSetQueueStack();
Demo4_GenericConstraints();
Demo5_CovarianceContravariance();
Demo6_IEnumerableVsIListVsICollection();

Console.WriteLine("===== 全部完成 =====");


// ------------------------------------------------------------
// 【1】List<T> vs 数组
// ------------------------------------------------------------
static void Demo1_ListVsArray()
{
    Console.WriteLine("【1】List<T> vs 数组");

    // 数组：长度创建时固定，不能动态增删
    int[] arr = new int[3] { 1, 2, 3 };
    
    // arr[3] = 4;   // 运行时抛 IndexOutOfRangeException（数组定长）
    Console.WriteLine($"  数组长度 = {arr.Length}（固定，不能动态扩容）");

    // List<T>：底层也是数组，容量不够自动扩容（通常翻倍）
    var list = new List<int> { 1, 2, 3 };
    list.Add(4);
    list.Add(5);
    list.RemoveAt(0);                                    // 删掉第一个
    Console.WriteLine($"  List 元素 = [{string.Join(", ", list)}]，Count={list.Count}, Capacity={list.Capacity}");

    Console.WriteLine("  结论：数组定长、性能最好；List 是动态数组，元素连续、按索引 O(1)，自动扩容。");
    Console.WriteLine();
}

// ------------------------------------------------------------
// 【2】Dictionary<K,V>（哈希表底层）
// ------------------------------------------------------------
static void Demo2_Dictionary()
{
    Console.WriteLine("【2】Dictionary<string,int>（哈希表）");

    var dict = new Dictionary<string, int>();
    dict["apple"] = 3;
    dict["banana"] = 5;
    dict["cherry"] = 7;

    // 索引器取值：key 定位 O(1)
    Console.WriteLine($"  dict[\"apple\"] = {dict["apple"]}（按 key 哈希定位，O(1)）");

    // TryGetValue：安全取值，key 不存在时不抛异常
    if (dict.TryGetValue("orange", out int count))
        Console.WriteLine($"  orange = {count}");
    else
        Console.WriteLine("  orange 不存在（TryGetValue 返回 false，不抛异常）");

    // key 唯一：重复赋值是覆盖旧值
    dict["apple"] = 100;
    Console.WriteLine($"  重复赋值后 apple = {dict["apple"]}（key 唯一，覆盖旧值）");

    // key 不能为 null（string 作 key 时）
    // dict[null!] = 1;   // 运行时会抛 ArgumentNullException：key 不能为 null

    Console.WriteLine("  结论：Dictionary 底层是哈希表，按 key 哈希值定位桶，平均 O(1)；key 唯一、无序。");
    Console.WriteLine();
}

// ------------------------------------------------------------
// 【3】HashSet / Queue / Stack
// ------------------------------------------------------------
static void Demo3_HashSetQueueStack()
{
    Console.WriteLine("【3】HashSet<T> / Queue<T> / Stack<T>");

    // HashSet：元素唯一、无序，自动去重
    var set = new HashSet<int> { 1, 2, 3, 3, 2, 1 };
    Console.WriteLine($"  HashSet 自动去重 = {{{string.Join(", ", set)}}}（重复的 3/2/1 只留一个）");
    Console.WriteLine($"  set.Contains(2) = {set.Contains(2)}（O(1) 查找）");

    // Queue：先进先出 FIFO
    var queue = new Queue<string>();
    queue.Enqueue("A");
    queue.Enqueue("B");
    queue.Enqueue("C");
    Console.WriteLine($"  Queue 出队顺序 = {queue.Dequeue()}, {queue.Dequeue()}, {queue.Dequeue()}（先进先出）");

    // Stack：后进先出 LIFO
    var stack = new Stack<string>();
    stack.Push("A");
    stack.Push("B");
    stack.Push("C");
    Console.WriteLine($"  Stack 弹出顺序 = {stack.Pop()}, {stack.Pop()}, {stack.Pop()}（后进先出）");

    Console.WriteLine("  结论：HashSet 去重+快速查找；Queue 先进先出（排队）；Stack 后进先出（撤销/递归）。");
    Console.WriteLine();
}
// ------------------------------------------------------------
// 【4】泛型约束（where T : ...）
// ------------------------------------------------------------
static void Demo4_GenericConstraints()
{
    Console.WriteLine("【4】泛型约束（where T : ...）");

    // 约束 T : class, new() —— Book 是引用类型且有公共无参构造
    var repo = new Repository<Book>();
    Book item = repo.Create();
    Console.WriteLine($"  约束 class,new()：创建 {item.Title}");

    // 约束 T : INamed —— 让编译器确定 T 有 Name 属性可调用
    var player = new Player { Name = "张三" };
    PrintName(player);                                   // T : INamed

    // 下面两处被注释掉，因为不满足约束会编译报错——取消注释看效果：
    // var repoInt = new Repository<int>();   // 错误：int 是值类型，不满足 class
    // PrintName(123);                        // 错误：int 没实现 INamed

    Console.WriteLine("  结论：泛型约束限制 T 的能力范围，让编译器知道 T 有哪些成员可调用，编译期就报错。");
    Console.WriteLine();
}

static void PrintName<T>(T item) where T : INamed
{
    Console.WriteLine($"  约束 INamed：名字 = {item.Name}");
}

// ------------------------------------------------------------
// 【5】协变 out / 逆变 in
// ------------------------------------------------------------
static void Demo5_CovarianceContravariance()
{
    Console.WriteLine("【5】协变 out / 逆变 in");

    // 协变（out）：子类型集合可以赋给父类型集合（"读出"方向安全）
    // IEnumerable<out T> 是协变的，所以 IEnumerable<Dog> 能赋给 IEnumerable<Animal>
    IEnumerable<Dog> dogs = new List<Dog> { new Dog { Name = "旺财" } };
    IEnumerable<Animal> animals = dogs;                  // 协变：Dog 集合当成 Animal 集合用
    Console.WriteLine($"  协变：Animal 集合第一个 = {animals.First().Name}（实际类型 {animals.First().GetType().Name}）");

    // 逆变（in）：父类型处理器可以赋给子类型委托（"消费"方向安全）
    // Action<in T> 是逆变的，Action<Animal> 能赋给 Action<Dog>
    Action<Animal> feedAnimal = a => Console.WriteLine($"    喂了 {a.Name}");
    Action<Dog> feedDog = feedAnimal;                    // 逆变：能喂 Animal 的，一定能喂 Dog
    feedDog(new Dog { Name = "旺财" });

    // List<T> 是不变的（invariant），下面会编译报错，注释掉：
    // List<Animal> animalList = new List<Dog>();   // 错误：类不支持协变/逆变

    Console.WriteLine("  结论：协变 out 用于\"产出\"（IEnumerable<T>），逆变 in 用于\"消费\"（Action<T>）；只有接口/委托的泛型参数可变。");
    Console.WriteLine();
}

// ------------------------------------------------------------
// 【6】IEnumerable vs ICollection vs IList
// ------------------------------------------------------------
static void Demo6_IEnumerableVsIListVsICollection()
{
    Console.WriteLine("【6】IEnumerable vs ICollection vs IList（接口层次）");

    // IEnumerable：只能遍历，没有 Add / 索引
    IEnumerable<int> onlyRead = new List<int> { 1, 2, 3 };
    // onlyRead.Add(4);   // 错误：IEnumerable 没有 Add 方法
    Console.WriteLine("  IEnumerable：只能 foreach 遍历，没有增删/索引");

    // ICollection：能增删、能取 Count
    ICollection<int> canAddRemove = new List<int> { 1, 2, 3 };
    canAddRemove.Add(4);                                  // ICollection 有 Add/Remove/Count
    Console.WriteLine($"  ICollection：可 Add，Count = {canAddRemove.Count}");

    // IList：能按索引读写
    IList<int> indexable = new List<int> { 1, 2, 3 };
    indexable[0] = 99;                                    // IList 支持 this[int] 索引读写
    indexable.Add(4);
    Console.WriteLine($"  IList：按索引读写 indexable[0] = {indexable[0]}");

    Console.WriteLine("  结论：IEnumerable 只遍历 → ICollection 增删/计数 → IList 按索引读写，逐层增强。");
    Console.WriteLine();
}


// ============================================================
// 下面是各 demo 用到的类 / 接口（全部放底部，顶级语句必须在类型声明之前）
// ============================================================

// —— 演示 泛型约束 ——
class Book
{
    public string Title { get; set; } = "C# 高级编程";
}

// Repository<T>：约束 T 是引用类型、有公共无参构造
class Repository<T> where T : class, new()
{
    public T Create() => new T();
}

interface INamed
{
    string Name { get; }
}

class Player : INamed
{
    public string Name { get; set; } = "";
}

// —— 演示 协变/逆变 ——
class Animal
{
    public string Name { get; set; } = "";
}


public record class tt(string Name);

class Dog : Animal { }
class Cat : Animal { }
