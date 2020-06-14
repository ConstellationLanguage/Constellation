namespace Constellation.Math
{
    public class MathNodeFactory: INodeGetter
    {
        public string GetNameSpace()
        {
            return NameSpace.NAME;
        }

        public Node<INode> GetNode(string nodeName)
        {
            switch (nodeName)
            {
                case Absolute.NAME:
                    INode nodeAbsolute = new Absolute() as INode;
                    return new Node<INode>(nodeAbsolute);
				case Approx.NAME:
                    INode nodeAprox = new Approx() as INode;
                    return new Node<INode>(nodeAprox);
				case ArcCos.NAME:
                    INode nodeArcCos = new ArcCos() as INode;
                    return new Node<INode>(nodeArcCos);
				case ArcSin.NAME:
                    INode nodeArcSin = new ArcSin() as INode;
                    return new Node<INode>(nodeArcSin);
				case ArcTan2.NAME:
                    INode nodeArcTan2 = new ArcTan2() as INode;
                    return new Node<INode>(nodeArcTan2);
				case Ceil.NAME:
                    INode nodeCeil = new Ceil() as INode;
                    return new Node<INode>(nodeCeil);
				case CeilToInt.NAME:
                    INode nodeCeilToInt = new CeilToInt() as INode;
                    return new Node<INode>(nodeCeilToInt);
				case Clamp.NAME:
                    INode nodeClamp = new Clamp() as INode;
                    return new Node<INode>(nodeClamp);
				case Clamp01.NAME:
                    INode nodeClamp01 = new Clamp01() as INode;
                    return new Node<INode>(nodeClamp01);
				case ClosestPowerOf2.NAME:
                    INode nodeClosestPowerOf2 = new ClosestPowerOf2() as INode;
                    return new Node<INode>(nodeClosestPowerOf2);
				case Cosinus.NAME:
                    INode nodeCosinus = new Cosinus() as INode;
                    return new Node<INode>(nodeCosinus);
				case DeltaAngle.NAME:
                    INode nodeDeltaAngle = new DeltaAngle() as INode;
                    return new Node<INode>(nodeDeltaAngle);
				case Exp.NAME:
                    INode nodeExp = new Exp() as INode;
                    return new Node<INode>(nodeExp);
				case Floor.NAME:
                    INode nodeFloor = new Floor() as INode;
                    return new Node<INode>(nodeFloor);
				case FloorToInt.NAME:
                    INode nodeFloorToInt = new FloorToInt() as INode;
                    return new Node<INode>(nodeFloorToInt);
				case Lerp.NAME:
                    INode nodeLerp = new Lerp() as INode;
                    return new Node<INode>(nodeLerp);
				case LerpAngle.NAME:
                    INode nodeLerpAngle = new LerpAngle() as INode;
                    return new Node<INode>(nodeLerpAngle);
				case Log10.NAME:
                    INode nodeLog10 = new Log10() as INode;
                    return new Node<INode>(nodeLog10);
				case Logarithm.NAME:
                    INode nodeLogarithm = new Logarithm() as INode;
                    return new Node<INode>(nodeLogarithm);
				case Sinus.NAME:
                    INode nodeSinus_ = new Sinus() as INode;
                    return new Node<INode>(nodeSinus_);
                case Max.NAME:
                    INode nodeMax = new Max() as INode;
                    return new Node<INode>(nodeMax);
                case Min.NAME:
                    INode nodeMin = new Min() as INode;
                    return new Node<INode>(nodeMin);
                case MoveTowards.NAME:
                    INode nodeMoveTowards = new MoveTowards() as INode;
                    return new Node<INode>(nodeMoveTowards);
                case MoveTowardsAngle.NAME:
                    INode nodeMoveTowardsAngle = new MoveTowardsAngle() as INode;
                    return new Node<INode>(nodeMoveTowardsAngle);
                case NextPowerOfTwo.NAME:
                    INode nodeNextPowerOfTwo = new NextPowerOfTwo() as INode;
                    return  new Node<INode>(nodeNextPowerOfTwo);
                case PerlinNoise.NAME:
                    INode nodePerlinNoise = new PerlinNoise() as INode;
                    return new Node<INode>(nodePerlinNoise);
                case PingPong.NAME:
                    INode nodePingPong = new PingPong() as INode;
                    return new Node<INode> (nodePingPong);
                case Pow.NAME:
                    INode nodePow = new Pow() as INode;
                    return new Node<INode>(nodePow);
                case SquareRoot.NAME:
                    INode nodeSqrt = new SquareRoot() as INode;
                    return new Node<INode>(nodeSqrt);
                case Tan.NAME:
                    INode nodeTan = new Tan() as INode;
                    return new Node<INode>(nodeTan);
                case Random.NAME:
                    INode nodeRandom = new Random() as INode;
                    return new Node<INode>(nodeRandom);
                default:
                    return null;
            }
        }
    }
}
