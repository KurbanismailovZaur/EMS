def get_distance(a, b):
    """
    Нахождения растояния по теореме Пифагора (Евклидовое растояние)
    """
    result = 0
    for j in range(3):
        result += (b[j] - a[j]) ** 2
    result **= 0.5

    return result