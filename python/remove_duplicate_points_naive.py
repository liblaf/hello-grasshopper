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

import rhinoscriptsyntax as rs


def point_less(u, v):
    u_coordinates = rs.PointCoordinates(u)
    v_coordinates = rs.PointCoordinates(v)
    return u_coordinates < v_coordinates


removed = set()
for u, v in itertools.combinations(P, 2):
    if u in removed or v in removed:
        continue
    if not rs.PointCompare(u, v, tolerance=t):
        continue
    removed.add(v if point_less(u, v) else u)
Q = [u for u in P if u not in removed]
