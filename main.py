import csv

import numpy as np
import matplotlib
import matplotlib.pyplot as plt
import seaborn as sns
x= []
y = []
val = []

with open('C:\\test\\data.csv', newline='') as csvfile:
    reader = csv.reader(csvfile)
    i = 0
    for row in reader:
        if i == 0:
            i = i +1
            continue
        x.append(int(row[0]))
        y.append(int(row[1]))
        val.append(int(row[2]))

x_ax = [1,   2,   4,  8, 16,  32,  64, 128, 256]
y_ax = [1, 2, 4, 8, 16]

t = val
val = np.array(val).reshape(5,9)

fig, [ax0, ax1, ax2] = plt.subplots(nrows=3, ncols=1,figsize=(10, 15))

ax2.set_title('Зависимость времени от размера блоков и кол-ва операций')

ax2 = sns.heatmap(val,xticklabels=x_ax, yticklabels=y_ax,linewidths=1, cmap="YlGnBu")
ax2.set_xlabel('blockSize')
ax2.set_ylabel('Opertaions')

ax0.set_title('Зависимость времени от размера блоков')
ax0.set_ylabel('Time')
ax0.set_xlabel('blockSize')
ax0.plot(x_ax, t[0:9])

ax1.set_title('Зависимость времени от кол-ва операций')
ax1.set_ylabel('Time')
ax1.set_xlabel('Operation')
ax1.plot(y_ax, val[0:,1], color='orange')

plt.savefig('img.png')
plt.show()
