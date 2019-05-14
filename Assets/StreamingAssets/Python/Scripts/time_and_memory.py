import psutil
from time import clock
from functools import reduce


def seconds_to_str(t):
    return "%d:%02d:%02d.%03d" % \
           reduce(lambda ll, b: divmod(ll[0], b) + ll[1:],
                  [(t * 1000,), 1000, 60, 60])


line = "=" * 40


def log(s, elapsed=None, memory=None):
    print()
    print(line)
    print(seconds_to_str(clock()), '-', s)
    if elapsed:
        print("Elapsed time:", elapsed)
    if memory:
        print("Memory used:", memory)
    print(line)
    print()


def endlog():
    p = psutil.Process()
    memory = '%smb' % round(p.memory_info()[0] / 1024 / 1024, 3)
    end = clock()
    elapsed = end - start
    log("End Program", seconds_to_str(elapsed), memory)


def now():
    return seconds_to_str(clock())


start = clock()
log("Start Program")
