# Git CLI 操作指南

> 从可视化 Git 工具（GitHub Desktop / SourceTree / TortoiseGit）切换到命令行，最大的障碍不是记命令，而是**把脑子里 GUI 的"画面"翻译成文字**。这份指南围绕这个痛点编写：先建立对应关系，再给速查表。

---

## 0. GUI 的按钮 = CLI 的一句话

| GUI 里的动作 | CLI 对应 |
|---|---|
| 查看当前改了哪些文件 | `git status` |
| 勾选要提交的文件 | `git add <文件>` |
| 填写提交信息并提交 | `git commit -m "..."` |
| 推送按钮 | `git push` |
| 拉取按钮 | `git pull` |
| 看提交历史图 | `git log --oneline --graph` |
| 新建分支 | `git switch -c <分支名>` |
| 切换分支 | `git switch <分支名>` |
| 合并分支 | `git merge <分支名>` |
| 丢弃某文件的修改 | `git restore <文件>` |
| 撤销上一次提交 | `git reset --soft HEAD~1` |

---

## 1. 一次性配置（只做一次）

```bash
git config --global user.name  "你的名字"
git config --global user.email "你的邮箱"
git config --global init.defaultBranch main   # 默认分支叫 main
```

> Windows 下在 Git Bash 里运行这些命令即可。

---

## 2. 四个最常用命令（记住这四个就能活）

```bash
git status            # 看状态：改了什么、哪些已暂存、在哪个分支
git add .             # 暂存所有改动（"打勾选中全部"）
git commit -m "说明"  # 提交（生成一个快照）
git push              # 推到远程仓库
```

**`git status` 是你最好的朋友**——每次操作前后都看一眼，能解决 80% 的困惑。

---

## 3. 提交工作流（GUI 里最常做的那套）

```bash
git status                      # 1. 看看改了什么
git diff                        # 2. （可选）看具体改动内容
git add 文件名                  # 3. 暂存某个文件；或 git add . 暂存全部
git commit -m "做了什么"        # 4. 提交
git push                        # 5. 推送
```

提交信息（commit message）建议用祈使句，比如 `修复登录页样式`、`添加用户模块`，而不是 `改了点东西`。

---

## 4. 分支（Branch）

```bash
git branch               # 列出本地分支，带 * 的是当前分支
git branch -a            # 包括远程分支
git switch -c 新分支名    # 新建并切换到该分支（= GUI 的 New Branch）
git switch 已有分支名      # 切换到已有分支
git branch -d 分支名      # 删除已合并的分支
git branch -D 分支名      # 强制删除（会丢未合并的提交，谨慎）
```

> 提示：旧教程常用 `git checkout` 来切分支，现在官方推荐用 `git switch`（切分支）和 `git restore`（还原文件），语义更清晰，不容易误操作。

---

## 5. 合并与冲突

```bash
git switch main             # 1. 先切到要"合入"的分支（一般是 main）
git merge feature-xxx       # 2. 把 feature-xxx 合并进来
```

**冲突时怎么办**（GUI 里会弹窗让你选，CLI 里要自己编辑文件）：

1. 冲突文件里会出现这种标记：
   ```
   <<<<<<< HEAD
   你的版本
   =======
   别人的版本
   >>>>>>> feature-xxx
   ```
2. 手动改成你想要的内容，删掉 `<<<<<<<` `=======` `>>>>>>>` 这三行标记
3. 然后：
   ```bash
   git add 冲突文件
   git commit        # 不需要 -m，会进入编辑器；直接保存退出即可
   ```

---

## 6. 撤销 / 回退（GUI 里最"神秘"的操作）

**分三种情况，对号入座：**

```bash
# ① 文件改了但还没 git add（想丢弃改动，回到上次提交的样子）
git restore 文件名

# ② 已经 git add 了，想"取消勾选"（不丢改动，只是取消暂存）
git restore --staged 文件名

# ③ 已经 commit 了，想撤销这次提交
git reset --soft HEAD~1    # 撤销提交，但改动保留在暂存区（最安全）
git reset --mixed HEAD~1   # 撤销提交，改动回到工作区（默认）
git reset --hard HEAD~1    # 撤销提交并彻底丢弃改动（危险！不可恢复）
```

> 记一个原则：**`--hard` 会真的丢东西**，用之前想清楚。改主意了想"反悔撤销"，用 `git reflog` 找回。

---

## 7. 查看历史（GUI 里的"历史"页面）

```bash
git log                          # 完整历史
git log --oneline                # 一行一条，简洁
git log --oneline --graph --all  # 带分支图的树状历史（强烈推荐！）
git log -p 文件名                # 某个文件的修改历史
git show 提交哈希                 # 看某次提交具体改了什么
```

建议给 `git log --oneline --graph --all` 配个别名（见第 10 节），效果接近 GUI 的历史图。

---

## 8. 暂存现场（Stash）

临时代码不想提交、但要先切去别的分支时用：

```bash
git stash              # 把改动暂存起来，工作区变干净
git stash pop          # 恢复暂存的改动
git stash list         # 看有哪些暂存
```

---

## 9. 远程相关

```bash
git clone 仓库地址          # 首次下载仓库
git pull                    # 拉取远程更新并合并（= 拉取按钮）
git fetch                   # 只下载远程更新，不合并（更谨慎）
git push                    # 推送本地提交
git remote -v               # 查看远程仓库地址
```

> `git pull` = `git fetch` + `git merge`。遇到"push 被拒绝，说远程有我没拉的新提交"时，先 `git pull` 再 `git push`。

---

## 10. 让 CLI 好用的别名（推荐配置）

```bash
git config --global alias.st status
git config --global alias.co "switch"
git config --global alias.br branch
git config --global alias.ci commit
git config --global alias.lg "log --oneline --graph --all"
git config --global alias.last "log -1 HEAD"
```

配完后，`git lg` 就是你想要的那个"历史树状图"。

---

## 推荐的最小上手路径

1. 先只练 `status → add → commit → push` 这四个，循环几天
2. 加练 `switch` 和 `merge`
3. 遇到"改错了想还原"，练 `restore` 和 `reset --soft`
4. 其他命令（rebase、cherry-pick、tag、reflog）等上面熟了再碰

---

## 速查卡片（打印/贴桌面用）

```
git status                      看状态
git add .                       暂存全部
git commit -m "说明"            提交
git push / git pull             推送 / 拉取

git switch -c 分支名             新建并切换分支
git switch 分支名                切换分支
git merge 分支名                 合并分支

git restore 文件                丢弃未暂存的改动
git restore --staged 文件        取消暂存
git reset --soft HEAD~1         撤销上次提交（保留改动）

git log --oneline --graph --all 历史树状图
git stash / git stash pop       暂存现场 / 恢复
```
