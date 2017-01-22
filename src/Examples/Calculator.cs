namespace Examples
{
    public class Calculator
    {
        //this is of course a horrible calculator, but this project is about bdd :)

        private int _firstNum;
        private int _secondNum;
        public void EnterFirstNum(int num)
        {
            this._firstNum = num;
        }

        public void EnterSecondNum(int num)
        {
            this._secondNum = num;
        }

        public int Add()
        {
            return this._firstNum + this._secondNum;
        }
    }
}
