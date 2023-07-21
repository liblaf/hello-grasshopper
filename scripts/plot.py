import matplotlib.pyplot as plt
import pandas as pd
import seaborn as sns

df: pd.DataFrame = pd.read_csv("data/benchmark.csv", index_col="N")

plt.figure(dpi=300)
sns.lineplot(data=df)
plt.xlabel("Number of Points")
plt.xscale("log", base=2)
plt.ylabel("Execution Time (ms)")
plt.yscale("log", base=2)
plt.tight_layout()
plt.savefig("assets/benchmark.pdf")
plt.close()
