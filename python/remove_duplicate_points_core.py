import sys
from array import array

import numpy as np
from scipy.spatial import KDTree

raw: array = array("f")
raw.frombytes(sys.stdin.buffer.read())
points: np.ndarray = np.array(raw[:-1]).reshape(-1, 3)
tolerance: float = raw[-1]
kdtree: KDTree = KDTree(data=points)
results: set[int] = set(range(points.shape[0]))
for i in np.lexsort(keys=points[:, ::-1].transpose()):
    if i not in results:
        continue
    duplicates: set[int] = (
        set(kdtree.query_ball_point(x=points[i], r=tolerance)) & results
    )
    min_id: int = min(duplicates, key=lambda x: tuple(points[x]))
    results -= set(duplicates)
    results.add(min_id)
print(results)
