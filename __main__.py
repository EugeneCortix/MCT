import numpy as np
import random

B = []
P = []
W1 = []
W2 = []
b1 = []
b2 = []
INPUT_DIM = 0
OUT_DIM = INPUT_DIM

H_DIM = 10
H_DIM2 = 10

def printout(vector):
    s=""
    for element in vector:
        s+= str(element) +'\n'
    with open("Ppredicted.txt", "a") as f:
        print(s, file=f)
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
            line = line.replace(',', '.')
            x.append(float(line))
            #x.append(ReturnValues(line.split()))

def relu(t):
    return np.maximum(t, 0)

def predict():
    t1 = B @ W1 + b1   #linear part
    h1 = relu(t1)       #activation
    t2 = h1 @ W2 + b2   #second layer
    h2 = relu(t2)
    t3 = h2 @ W3 + b3
    return t3
def to_full_batch(y, num_classes):
    y_full = np.zeros((len(y), num_classes))
    for j, yj in enumerate(y):
        y_full[j, yj] = 1
    return y_full

def relu_deriv(t):
    return (t >= 0).astype(float)

def read_dataset():
    dset = []
    for i in range (1, 1):
        Pd = []
        Bd = []

        filename = "dataset\\x" + str(i) +".txt"
        read(Pd, filename)
        filename = "dataset\\b" + str(i) + ".txt"
        read(Bd, filename)
        dspair = []
        dspair.append(Pd)
        dspair.append(Bd)
        dset.append(np.array(dspair))
    return dset


read(B, "B.txt")
dataset = read_dataset()
INPUT_DIM = len(B)
OUT_DIM = len(B)
W1 = np.random.randn(INPUT_DIM, H_DIM)
W2 = np.random.randn(H_DIM, H_DIM2)
W3 = np.random.randn(H_DIM2, OUT_DIM)
b1 = np.random.randn(H_DIM)
b2 = np.random.randn(H_DIM2)
b3 = np.random.randn(OUT_DIM) #.reshape(OUT_DIM, 1)
ALPHA = 0.0002
NUM_EPOCHS = 400
BATCH_SIZE = 50
for ep in range(NUM_EPOCHS):
    random.shuffle(dataset)
    for i in range(len(dataset) // BATCH_SIZE):

        batch_x, batch_y = zip(*dataset[i*BATCH_SIZE : i*BATCH_SIZE+BATCH_SIZE])
        x = np.concatenate(batch_x, axis=0)
        y = np.array(batch_y)

        # Forward
        t1 = B @ W1 + b1
        h1 = relu(t1)
        t2 = h1 @ W2 + b2
        h2 = relu(t2)
        t3 = h2 @ W3 + b3
       # z = softmax_batch(t2)
       # E = np.sum(sparse_cross_entropy_batch(z, y))

        # Backward
        y_full = to_full_batch(y, OUT_DIM)
        dE_dt2 = t3 - y_full
        dE_dW2 = h1.T @ dE_dt2
        dE_db2 = np.sum(dE_dt2, axis=0, keepdims=True)
        dE_dh1 = dE_dt2 @ W2.T
        dE_dt1 = dE_dh1 * relu_deriv(t1)
        dE_dW1 = x.T @ dE_dt1
        dE_db1 = np.sum(dE_dt1, axis=0, keepdims=True)

        # Update
        W1 = W1 - ALPHA * dE_dW1
        b1 = b1 - ALPHA * dE_db1
        W2 = W2 - ALPHA * dE_dW2
        b2 = b2 - ALPHA * dE_db2

probs = predict()
printout(probs)
