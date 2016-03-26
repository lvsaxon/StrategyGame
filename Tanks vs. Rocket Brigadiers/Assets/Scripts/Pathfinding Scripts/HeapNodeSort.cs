
using UnityEngine;
using System;
using System.Collections;

public class HeapNodeSort<T> where T: IHeapItem<T> {

    T[] nodeItems;   //Array of Type T
    int itemCount;

    public HeapNodeSort(int maxHeapSize) {
        nodeItems = new T[maxHeapSize];
    }


    /* Add item into Heap */
    public void Add(T addItems) {
        addItems.HeapIndex = itemCount;
        nodeItems[itemCount] = addItems;
        SortUp(addItems);
        itemCount++;
    }


    /* Update Item */
    public void UpdateNodeItem(T item) {
        SortUp(item);
    }


    /* Total Heap Amount */
    public int Count() {

        return itemCount;
    }

    /* Search Heap For specific Node*/
    public bool Contains(T item) {

        return Equals(nodeItems[item.HeapIndex], item);
    }


    /* Remove 1st Item from Heap */
    public T RemoveFirstItem() {
        T firstItem = nodeItems[0];
        itemCount--;

        //Last Item now = First; Then Sort Down
        nodeItems[0] = nodeItems[itemCount];
        nodeItems[0].HeapIndex = 0;
        SortDown(nodeItems[0]);

        return firstItem;
    }


    /* Sort Node Down if Node has Lower Priority of Higher F-Cost than Children */
    void SortDown(T item) {
        
        while(true) {
            int leftChildIndex = (2 * item.HeapIndex) + 1;
            int rightChildIndex = (2 * item.HeapIndex) + 2;
            int swapIndex = 0;

            //Check If leftChild Index is < Total HeapItem Amount
            if(leftChildIndex < itemCount) {
               swapIndex = leftChildIndex;
                
                if(rightChildIndex < itemCount) {
                    if(nodeItems[leftChildIndex].CompareTo(nodeItems[rightChildIndex]) < 0){
                       swapIndex = rightChildIndex;
                    }
                }

                if(item.CompareTo(nodeItems[swapIndex]) < 0) 
                   SwapItems(item, nodeItems[swapIndex]);
                else
                    return;

            }else
                return;
        }
    }


    /* Sort Node Up; parent = (n-1)/2;  leftChild = 2n; RightChild = 2n+1*/
    void SortUp(T item) {
        int parentIndex = (item.HeapIndex-1)/2;

        while(true) {
            T parentItem = nodeItems[parentIndex];
            if(item.CompareTo(parentItem) > 0) {
               SwapItems(item, parentItem);
            }else
                break;

            parentIndex = (item.HeapIndex-1)/2;
        }
    }


    /* Swap Nodes with Lower F-Costs than Its Parent Node*/
    void SwapItems(T node, T node2) {
        int tempNodeIndex;
        
        nodeItems[node.HeapIndex] = node2;
        nodeItems[node2.HeapIndex] = node;
        tempNodeIndex = node.HeapIndex;

        node.HeapIndex = node2.HeapIndex;
        node2.HeapIndex = tempNodeIndex;
    }
}


public interface IHeapItem<T>: IComparable<T> {
    int HeapIndex {
        set;
        get;
    }
}