import numpy
import numpy as np
from sklearn.linear_model import LinearRegression

A = []
x = []
b = []
W1 = []
W2 = []
b1 = []
b2 = []
INPUT_DIM = 0
OUT_DIM = INPUT_DIM

H_DIM = 10
def ReturnValues(splitLine):
    lineEnd = []
#    splitLine = splitLine.replace(',', '.')
    for element in splitLine:
        element = element.replace(',', '.')
        lineEnd.append(float(element))
    return lineEnd
def read(x, waytofile):
    with open(waytofile, 'r') as f:   # read matrix file
        for line in f:
            x.append(ReturnValues(line.split()))

def relu(t):
    return np.maximum(t, 0)

def normalize():
    a = np.array(A)
    mean = a.mean(axis = 0)
    a -= mean


def softmax(t):             #Get probability vector
    out = np.exp(t)
    return out / np.sum(out)

def predict():
    for i in range(len(A)):
        A[i].append(b[0][i])
   # A.append(b[0])
    Aa = np.array(A)
    t1 = Aa @ W1 + b1   #linear part
    h1 = relu(t1)       #activation
    t2 = h1 @ W2 + b2   #second layer
    h2 = relu(t2)
    t3 = h2 @ W3
    t3 += b3
    #z = softmax(t2)
    return t3

def read_dataset():
    dset = []
    for i in range (1, 15):
        Ad = []
        bd = []
        xd =[]

        filename = "dataset\\A" + str(i) +".txt"
        read(Ad, filename)
        filename = "dataset\\b" + str(i) + ".txt"
        read(bd, filename)
        filename = "dataset\\x" + str(i) + ".txt"
        read(xd, filename)
        for j in range(len(Ad)):
            Ad[j].append(b[0][j])
        dspair = []
        dspair.append(Ad)
        dspair.append(xd)
        dset.append(np.array(dspair))
    return dset


read(A, "A16.txt")
read(b, "b16.txt")
dataset = read_dataset()
INPUT_DIM = len(A[0])
OUT_DIM = len(b[0])
W1 = np.random.randn(INPUT_DIM + 1, H_DIM + 1)
W2 = np.random.randn(H_DIM + 1, OUT_DIM)
W3 = np.random.randn(OUT_DIM, 1)
b1 = np.random.randn(H_DIM + 1)
b2 = np.random.randn(OUT_DIM)
b3 = np.random.randn(OUT_DIM).reshape(OUT_DIM, 1)
probs = predict()
print(probs)
