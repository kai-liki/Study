package study;

import javax.swing.JFrame;
import javax.media.j3d.*;
import javax.vecmath.*;
import java.awt.*;
import javax.swing.*;
import com.sun.j3d.utils.*;
import com.sun.j3d.utils.universe.*;
import com.sun.j3d.utils.geometry.*;

public class ByMyself extends JFrame {
	private SimpleUniverse su;
	private BranchGroup objRoot;
	private JPanel drawingPanel;
	
	private BranchGroup createSenceGraph(){
		BranchGroup bgObj=new BranchGroup();
		
		//TransformGroup1
		TransformGroup tg=new TransformGroup();
		tg.setCapability(TransformGroup.ALLOW_TRANSFORM_WRITE);
		ColorCube cc=new ColorCube(0.5);
		tg.addChild(cc);
		bgObj.addChild(tg);
		Alpha alpha=new Alpha(-1,5000);
		Transform3D yAxis=new Transform3D();
		ScaleInterpolator si=new ScaleInterpolator(alpha,tg,yAxis,0.1f,1.0f);
		BoundingSphere bs=new BoundingSphere(new Point3d(0.0,0.0,0.0),100);
		si.setSchedulingBounds(bs);
		bgObj.addChild(si);
		
		//TransformGroup2
		TransformGroup tg2=new TransformGroup();
		tg2.setCapability(TransformGroup.ALLOW_TRANSFORM_WRITE);
		Text2D txt2d=new Text2D("Hello text", new Color3f(0.0f,0.0f,1.0f), "Hello name", 100, 1);
		tg2.addChild(txt2d);
		bgObj.addChild(tg2);
		si.setTarget(tg2);
		
		bgObj.compile();
		return bgObj;
	}
	
	private Canvas3D createUniverse(){
		GraphicsConfiguration gc=SimpleUniverse.getPreferredConfiguration();
		Canvas3D canvas=new Canvas3D(gc);
		su=new SimpleUniverse(canvas);
		su.getViewingPlatform().setNominalViewingTransform();
		su.getViewer().getView().setMinimumFrameCycleTime(5);
		return canvas;
	}
	public ByMyself()
	{
		drawingPanel=new JPanel();
        setDefaultCloseOperation(javax.swing.WindowConstants.EXIT_ON_CLOSE);
        setTitle("HelloUniverse");
		drawingPanel.setLayout(new java.awt.BorderLayout());
		drawingPanel.setPreferredSize(new Dimension(255,255));
		this.getContentPane().add(drawingPanel,java.awt.BorderLayout.CENTER);
		this.pack();
		drawingPanel.add(createUniverse(),java.awt.BorderLayout.CENTER);
		objRoot=createSenceGraph();
		su.addBranchGraph(objRoot);
	}

	/**
	 * @param args
	 */
	public static void main(String[] args) {
		// TODO Auto-generated method stub
        java.awt.EventQueue.invokeLater(new Runnable() {
            public void run() {
                new ByMyself().setVisible(true);
            }
        });
	}

}
