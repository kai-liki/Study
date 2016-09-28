package hello;

public class HelloWorldMain {

	/**
	 * @param args
	 */
	public static void main(String[] args) {
		Test test=new MyTest().tryGetTests(1);
		System.out.println(test.getName());
	}

}
