"""
remove_duplicate_points

Removes similar points from a list

Inputs:
    P (Point)  : list of points to clean
    t (Number) : points closer than this distance will be combined

Output:
    Q (Point) : list of unique points
"""

import itertools
import subprocess
from array import array

import rhinoscriptsyntax as rs

SRC = """
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
"""

arr = array("f")
arr.fromlist(itertools.chain(*map(rs.PointCoordinates, P)))
arr.append(t)
startupinfo = subprocess.STARTUPINFO()
startupinfo.dwFlags = subprocess.STARTF_USESTDHANDLES | subprocess.STARTF_USESHOWWINDOW
process = subprocess.Popen(
    args=["python", "-c", SRC],
    stdin=subprocess.PIPE,
    stdout=subprocess.PIPE,
    stderr=subprocess.PIPE,
    startupinfo=startupinfo,
)
stdout, stderr = process.communicate(arr.tostring())
output = eval(stdout)
Q = [P[i] for i in output]
