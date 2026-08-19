# Day 3 当日清单：集合与泛型

> 对应总计划《第一周 · Day 3》。当天目标：把「List/Dictionary/HashSet/Queue/Stack、泛型约束、协变 out/逆变 in、IEnumerable vs IList vs ICollection」全部吃透，三个 ⭐ 高频题能口述 + 手写。
> 配套练习代码：`Day03-练习代码/`（`cd Day03-练习代码 && dotnet run`）

---

## 一、当天要搞懂的 6 个知识点

| # | 知识点 | 一句话核心 | 重点动作 |
|---|---|---|---|
| 1 | `List<T>` vs 数组 | 数组定长性能最好，List 是可变长动态数组 | 跑 demo 看自动扩容 |
| 2 | `Dictionary<K,V>` | 哈希表，key 定位 O(1) 查找 | 跑 demo 看取值/去重 |
| 3 | `HashSet`/`Queue`/`Stack` | 去重集合 / 先进先出 / 后进先出 | 跑 demo 看出入顺序 |
| 4 | 泛型约束 | `where T : ...` 限制类型参数能力 | 跑 demo 看约束语法 |
| 5 | 协变 `out` / 逆变 `in` | out 产出方向、in 消费方向，只有接口/委托可变 | 跑 demo 看赋值方向 |
| 6 | `IEnumerable` vs `ICollection` vs `IList` | 只遍历 → 可增删 → 按索引读写，逐层增强 | 跑 demo 看能力差异 |

---

## 二、知识拆解（含面试答案要点）

### 1. `List<T>` 和数组的区别 ⭐

- **数组（`T[]`）**：长度在创建时就固定，不能动态增删元素；元素在内存中连续存放，按索引访问是 O(1)，性能最好。
- **`List<T>`**：本质是"可变长数组"，底层也是一个数组，元素**连续存放**、按索引 O(1)；当容量不够时会**自动扩容**（通常翻倍，把旧元素拷贝到新数组）。

**面试答案要点：**
> 数组长度固定、创建后不能增删，性能最好；`List<T>` 是动态数组，底层也是数组、自动扩容。需要确定数量的固定数据用数组；需要频繁增删、数量不定的用 `List<T>`。两者都是强类型（泛型），避免了 `ArrayList` 装 object 的装箱问题。

### 2. `Dictionary<TKey,TValue>` 底层原理 ⭐

- 底层是**哈希表（HashTable）**：通过 `key.GetHashCode()` 算出哈希值，定位到某个"桶（bucket）"，再在桶里比较是否相等。
- **平均 O(1)** 的查找/插入/删除；key **唯一**（重复赋值是覆盖）、**无序**；key 不能为 `null`（string 作 key 时）。
- 冲突处理：多个 key 哈希值落到同一个桶时用**链地址法**（同一桶挂链表/红黑树）。

**面试答案要点：**
> Dictionary 底层是哈希表，用 key 的哈希值定位桶，平均 O(1) 查找。key 必须唯一、无序、不能为 null。查找快但遍历无序、比数组多花内存（存哈希+桶结构）。频繁按键查找用 Dictionary，顺序遍历用 List。

### 3. `HashSet<T>` / `Queue<T>` / `Stack<T>`

- **`HashSet<T>`**：元素**唯一**、**无序**、O(1) 查找，自动去重（重复添加无效）。
- **`Queue<T>`**：**先进先出（FIFO）**，`Enqueue` 入队、`Dequeue` 出队，像排队。
- **`Stack<T>`**：**后进先出（LIFO）**，`Push` 压栈、`Pop` 弹栈，像叠盘子，常用于撤销、递归、括号匹配。

### 4. 泛型约束（`where T : ...`）

- 作用：限制类型参数 `T` 的范围，让编译器**知道 T 有哪些成员可以调用**，否则只能用 `object` 的成员。
- 常见约束：`where T : class`（引用类型）、`where T : struct`（值类型）、`where T : new()`（有公共无参构造）、`where T : BaseClass`（基类）、`where T : IInterface`（接口）。
- 好处：**编译期类型安全** + 避免装箱，把"运行时才报错"提前到"编译期报错"。

### 5. 协变 `out` / 逆变 `in` ⭐

**核心对照表：**

| 维度 | 协变（`out`） | 逆变（`in`） |
|---|---|---|
| 方向 | 子类型 → 父类型（"产出"） | 父类型 → 子类型（"消费"） |
| 典型 | `IEnumerable<out T>`、`Func<out TResult>` | `Action<in T>`、`IComparer<in T>` |
| 语义 | 子类集合能当父类集合用 | 处理父类的方法能处理子类 |
| 允许的位置 | 只作返回值/`out` 参数 | 只作参数/`in` 参数 |
| 谁可变 | 仅**接口和委托**的泛型参数 | 仅**接口和委托**的泛型参数 |

**面试答案要点：**
> 协变 `out` 让子类型泛型能赋给父类型泛型（如 `IEnumerable<Dog>` → `IEnumerable<Animal>`），因为"能读出的都是安全的"；逆变 `in` 让父类型泛型能赋给子类型泛型（如 `Action<Animal>` → `Action<Dog>`），因为"能处理父类的必能处理子类"。只有接口/委托的泛型参数可变，类（如 `List<T>`）是不变的（invariant）。

### 6. `IEnumerable<T>` vs `ICollection<T>` vs `IList<T>`

- **`IEnumerable<T>`**：最小的集合契约，只提供**遍历**（`foreach` / `GetEnumerator`），没有增删改。
- **`ICollection<T>`**：继承 `IEnumerable<T>`，增加 `Add`/`Remove`/`Count`/`Contains` 等。
- **`IList<T>`**：继承 `ICollection<T>`，增加**按索引读写**（`this[int]` / `IndexOf` / `Insert`）。

> 三者是**逐层增强**的接口层次：只读遍历用 `IEnumerable`，要增删用 `ICollection`，要按索引随机访问用 `IList`。作为方法参数时，用**最小能满足需求**的接口（能 `IEnumerable` 就不 `IList`），降低耦合。

---

## 三、手写练习（当日必做）

对应 `Program.cs`，每个都要能**自己手写一遍**（不照抄）：

- [ ] 写一个定长数组和一个 `List<T>`，演示数组不能增删、List 能 `Add`/`RemoveAt`
- [ ] 写一个 `Dictionary<string,int>`，用索引器取值 + `TryGetValue` 安全取值
- [ ] 写 `HashSet`（自动去重）、`Queue`（先进先出）、`Stack`（后进先出）各一个
- [ ] 写一个带 `where T : class, new()` 约束的泛型类，和一个 `where T : I接口` 的泛型方法
- [ ] 写协变 `IEnumerable<Dog>` → `IEnumerable<Animal>`、逆变 `Action<Animal>` → `Action<Dog>` 各一个
- [ ] 用 `IEnumerable`/`ICollection`/`IList` 三个接口各声明一个引用，对比能调用的成员

---

## 四、口述自测（当天结尾，用「结论 → 原理 → 例子」讲出来）

1. **`List` 和数组的区别？**

   数组长度创建时固定、不能增删、性能最好；`List<T>` 是动态数组，底层也是数组、容量不够自动翻倍扩容。两者都是强类型，避免了 `ArrayList` 存 `object` 的装箱。确定数量用数组，频繁增删用 `List<T>`。

2. **`Dictionary` 底层原理？**

   底层是哈希表：用 `key.GetHashCode()` 算出哈希值定位到桶，再在桶里比较是否相等，平均 O(1) 查找。key 唯一、无序、不能为 null；冲突用链地址法解决。按键查找用 Dictionary，顺序遍历用 List。

3. **泛型协变和逆变是什么？**

   协变 `out`（`IEnumerable<out T>`）让子类型泛型赋给父类型泛型，安全因为只"读出"；逆变 `in`（`Action<in T>`）让父类型泛型赋给子类型泛型，安全因为只"消费"。只有接口/委托的泛型参数可变，类（如 `List<T>`）是不变的。

4. **（扩展）`IEnumerable`、`ICollection`、`IList` 的关系？**

   三者是逐层增强的接口：`IEnumerable` 只能遍历，`ICollection` 增加增删/计数，`IList` 再增加按索引读写。方法参数用最小能满足需求的接口，降低耦合。

---

## 五、完成标志 ✅

- [ ] 6 个知识点都能用自己的话讲出「一句话核心」
- [ ] `dotnet run` 跑通全部 6 个 demo，能解释每段输出
- [ ] 3 道 ⭐ 高频题能口述 + 手写关键代码
- [ ] 当天代码 `git add` + `commit` + `push`（养成习惯）
