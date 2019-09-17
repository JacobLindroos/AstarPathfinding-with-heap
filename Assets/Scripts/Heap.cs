using UnityEngine;
using System;

public class Heap<T> where T : IHeapItem<T>
{
	//array of type T
	T[] items;
	//keep track of our item count
	int currentIteamCount;

	/// <summary>
	/// Constructor Heap
	/// </summary>
	/// <param name="maxHeapSize"> of the array </param>
	public Heap(int maxHeapSize)
	{
		//array items is set to be equal to a new array with the int value of maxheapsize, maxheapsize represent the max of nodes from the nodegrid
		items = new T[maxHeapSize];
	}

	/// <summary>
	/// Adding items into the heap
	/// </summary>
	/// <param name="item"> to add to the heap </param>
	public void Add(T item)
	{
		//keeps track of the count of iteams in the heap
		item.HeapIndex = currentIteamCount;
		//sets a specific item equal to a int count value
		items[currentIteamCount] = item;
		//calls the method sortUp
		SortUp(item);
		//adds an int value to currentItemCount 
		currentIteamCount++;
	}

	/// <summary>
	/// Removes the first item in the heap
	/// </summary>
	/// <returns> removed item </returns>
	public T RemoveFirstItemOfHeap()
	{
		//creates a new item w. id firstItem and sets it equal to the item at index 0 in the heap
		T firstItem = items[0];
		//subtracts a in value from the item count
		currentIteamCount--;
		//takes the item in the end of the heap and puts it in the first place
		items[0] = items[currentIteamCount];
		//sets the heapindex of that item to 0
		items[0].HeapIndex = 0;
		//calls the method sortdown that takes item at index 0 in items heap
		SortDown(items[0]);
		//returns the firstitem
		return firstItem;

	}

	/// <summary>
	/// Change prio of an item
	/// </summary>
	/// <param name="item"> we want to update </param>
	public void UpdateItem(T item)
	{
		//we might find a node in the openSet we want to update with a lower Fcost, beacuse we found another path to it. We need to be able to update it´s position in the heap
		SortUp(item);
	}

	/// <summary>
	/// Returns the int count at any given moment
	/// </summary>
	public int Count
	{
		get { return currentIteamCount; }
	}

	/// <summary>
	/// Check if the heap contains a specific item
	/// </summary>
	/// <param name="item"> to check for in heap </param>
	/// <returns></returns>
	public bool Contains(T item)
	{
		//returns true/false if two items in the array are equal 
		return Equals(items[item.HeapIndex], item);
	}

	/// <summary>
	/// Sorts an item in heap comparing to it´s child items
	/// </summary>
	/// <param name="item"> to compare w. child item </param>
	private void SortDown(T item)
	{
		while (true)
		{
			//creates two int variables that gets the index of the items two children on the left and the right
			int childIndexLeft = item.HeapIndex * 2 + 1;
			int childIndexRight = item.HeapIndex * 2 + 2;
			//int variable for swapping
			int swapIndex = 0;

			//compares IF the index of the childLeft is less then the current item index
			if(childIndexLeft < currentIteamCount)
			{
				//swapIndex is set to the index of left child
				swapIndex = childIndexLeft;

				//compares IF the index of the right child is less then the current item index
				if(childIndexRight < currentIteamCount)
				{
					//checks if the left OR right item got the higher prio
					//IF left child got a lower prio then right child, change swapIndex to childindexright
					if(items[childIndexLeft].CompareTo(items[childIndexRight]) < 0)
					{
						swapIndex = childIndexRight;
					}
				}
				//swapIndex is equal to the child w. the highest prio
				//Check IF it´s true that parent item got a lower prio then it´s highest prio child item
				if(item.CompareTo(items[swapIndex]) < 0)
				{
					//swap the child item w. the parent item
					Swap(item, items[swapIndex]);
				}
				else //ELSE the parent item got a higher prio then the two children item, then parent item is in it´s right place. Exit the loop
				{
					return;
				}
			}
			else //ELSE the parent item dosn´t got any children item, then parent item is in it´s right place. Exit the loop
			{
				return;
			}
		}
	}

	/// <summary>
	/// Sorts the heap
	/// </summary>
	/// <param name="item">  </param>
	private void SortUp(T item)
	{
		//gets the parent index 
		int parentIndex = (item.HeapIndex - 1) / 2;

		//while-loop is true
		while (true)
		{
			//creates a variable T for our parentitem which is equal to the item with the parentindex
			T parentItem = items[parentIndex];
			//compares the current item w. the parentitem
			//if it got a higher prio it returns 1, same prio returns 0 and lower prio returns -1. So if it´s true that item got a higher prio then our parentitem, which means in our case it got a lower Fcost, then we swap it with the parentitem
			if(item.CompareTo(parentItem) > 0)
			{
				//swapping the child item w. it´s parent item
				Swap(item, parentItem);
			}
			else
			{
				break;
			}
			//recalculates the parentindex for another swapcheck
			parentIndex = (item.HeapIndex - 1) / 2;
		}
	}

	/// <summary>
	/// Swapping nodes in heap
	/// </summary>
	/// <param name="itemA"> to swap with itemB </param>
	/// <param name="itemB"> to swap with itemA </param>
	private void Swap(T itemA, T itemB)
	{
		//swapping the items in the array
		items[itemA.HeapIndex] = itemB;
		items[itemB.HeapIndex] = itemA;
		
		//swapping the items heapindex values
		int itemAIndex = itemA.HeapIndex;
		itemA.HeapIndex = itemB.HeapIndex;
		itemB.HeapIndex = itemAIndex;
	}
}

/// <summary>
/// Item in heap can now get/set a int heapIndex
/// </summary>
/// <typeparam name="T"> item of the heap </typeparam>
public interface IHeapItem<T> : IComparable<T>
{
	//gets and sets an int property
	int HeapIndex
	{
		get;
		set;
	}
}

