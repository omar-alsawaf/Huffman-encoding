using System;
using System.Collections.Generic;
using System.IO;

namespace FileCompression
{
    public class HuffmanNode : IComparable<HuffmanNode>
    {
        public byte Symbol { get; set; }
        public int Frequency { get; set; }
        public HuffmanNode Left { get; set; }
        public HuffmanNode Right { get; set; }

        public int CompareTo(HuffmanNode other)
        {
            return Frequency - other.Frequency;
        }
    }

    public class HuffmanHeap
    {
        private readonly List<HuffmanNode> heap;

        public HuffmanHeap()
        {
            heap = new List<HuffmanNode>();
        }

        public void Insert(HuffmanNode node)
        {
            heap.Add(node);
            int index = heap.Count - 1;

            while (index > 0)
            {
                int parentIndex = (index - 1) / 2;
                if (heap[index].CompareTo(heap[parentIndex]) >= 0)
                    break;

                Swap(index, parentIndex);
                index = parentIndex;
            }
        }

        public HuffmanNode ExtractMin()
        {
            if (heap.Count == 0)
                return null;

            HuffmanNode minNode = heap[0];
            int lastIndex = heap.Count - 1;
            heap[0] = heap[lastIndex];
            heap.RemoveAt(lastIndex);

            int index = 0;
            while (true)
            {
                int leftChildIndex = 2 * index + 1;
                int rightChildIndex = 2 * index + 2;
                int smallestChildIndex = index;

                if (leftChildIndex < heap.Count && heap[leftChildIndex].CompareTo(heap[smallestChildIndex]) < 0)
                    smallestChildIndex = leftChildIndex;

                if (rightChildIndex < heap.Count && heap[rightChildIndex].CompareTo(heap[smallestChildIndex]) < 0)
                    smallestChildIndex = rightChildIndex;

                if (smallestChildIndex == index)
                    break;

                Swap(index, smallestChildIndex);
                index = smallestChildIndex;
            }

            return minNode;
        }

        private void Swap(int index1, int index2)
        {
            HuffmanNode temp = heap[index1];
            heap[index1] = heap[index2];
            heap[index2] = temp;
        }

        public int Count
        {
            get { return heap.Count; }
        }
    }

    public class HuffmanCompression
    {
        public static byte[] Compress(byte[] input)
        {
            Dictionary<byte, int> frequencyTable = BuildFrequencyTable(input);
            HuffmanNode root = BuildHuffmanTree(frequencyTable);
            Dictionary<byte, string> codeTable = BuildCodeTable(root);

            List<bool> compressedBits = new List<bool>();
            foreach (byte symbol in input)
            {
                string code = codeTable[symbol];
                foreach (char bit in code)
                {
                    compressedBits.Add(bit == '1');
                }
            }

            return ConvertToBytes(compressedBits);
        }

        private static Dictionary<byte, int> BuildFrequencyTable(byte[] input)
        {
            Dictionary<byte, int> frequencyTable = new Dictionary<byte, int>();

            foreach (byte symbol in input)
            {
                if (frequencyTable.ContainsKey(symbol))
                    frequencyTable[symbol]++;
                else
                    frequencyTable[symbol] = 1;
            }

            return frequencyTable;
        }

        private static HuffmanNode BuildHuffmanTree(Dictionary<byte, int> frequencyTable)
        {
            HuffmanHeap minHeap = new HuffmanHeap();

            foreach (var entry in frequencyTable)
            {
                HuffmanNode node = new HuffmanNode { Symbol = entry.Key, Frequency = entry.Value };
                minHeap.Insert(node);
            }

            while (minHeap.Count > 1)
            {
                HuffmanNode left = minHeap.ExtractMin();
                HuffmanNode right = minHeap.ExtractMin();

                HuffmanNode parent = new HuffmanNode
                {
                    Frequency = left.Frequency + right.Frequency,
                    Left = left,
                    Right = right
                };

                minHeap.Insert(parent);
            }

            return minHeap.ExtractMin();
        }

        private static Dictionary<byte, string> BuildCodeTable(HuffmanNode root)
        {
            Dictionary<byte, string> codeTable = new Dictionary<byte, string>();
            BuildCodeTableRecursive(root, "", codeTable);
            return codeTable;
        }

        private static void BuildCodeTableRecursive(HuffmanNode node, string code, Dictionary<byte, string> codeTable)
        {
            if (node.Left == null && node.Right == null)
            {
                codeTable[node.Symbol] = code;
                return;
            }

            if (node.Left != null)
                BuildCodeTableRecursive(node.Left, code + "0", codeTable);

            if (node.Right != null)
                BuildCodeTableRecursive(node.Right, code + "1", codeTable);
        }

        private static byte[] ConvertToBytes(List<bool> bits)
        {
            int numBytes = (bits.Count + 7) / 8;
            byte[] bytes = new byte[numBytes];

            for (int i = 0; i < bits.Count; i++)
            {
                if (bits[i])
                    bytes[i / 8] |= (byte)(1 << (7 - (i % 8)));
            }

            return bytes;
        }

        public static void Main(string[] args)
        {
            string inputFilePath = "input.txt";
            string outputFilePath = "compressed.huff";

            try
            {
                // Read input from a text file
                byte[] input = File.ReadAllBytes(inputFilePath);

                // Compress the input
                byte[] compressed = Compress(input);

                // Write the compressed data to a file
                File.WriteAllBytes(outputFilePath, compressed);

                Console.WriteLine($"Original Size: {input.Length} bytes");
                Console.WriteLine($"Compressed Size: {compressed.Length} bytes");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error: {ex.Message}");
            }
        }
    }
}