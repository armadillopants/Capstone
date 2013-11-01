
public class GeomRect {
	public float x1;
	public float x2;
	public float y1;
	public float y2;
	
	public bool RectInside(GeomRect rect){
		return (this.PointInside(rect.x1, rect.y1) ||
				this.PointInside(rect.x2, rect.y2) ||
				this.PointInside(rect.x1, rect.y1) ||
				this.PointInside(rect.x2, rect.y2));
	}
	
	private bool PointInside(float x, float y){
		return (( x <= this.x2) &&
				( y <= this.y2) &&
				( x >= this.x1) &&
				( y >= this.y1));
	}
}
