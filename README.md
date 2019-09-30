# Automic-LOD
Automic poly reducer

高模运行时转低模

http://dev.gameres.com/Program/Visual/3D/PolygonReduction.htm 中文

http://dev.gameres.com/Program/Visual/3D/PolygonReduction.pdf english

# 使用方法

将AutomaticLOD作为componnet添加到gameobject上

把想要减面的mesh拖到meshToGenerate变量中

运行游戏

# 注意

此工程只是为了理解论文实现，仅适用于学习，还不具备实用性

目前默认位置相同，即为一个顶点

如果存在相同位置有多个顶点的情况（unity默认几何体在边缘存在这种情况），还需特殊处理

