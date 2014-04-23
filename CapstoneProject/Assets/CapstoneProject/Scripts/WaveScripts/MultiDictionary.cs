using System.Collections;
using System.Collections.Generic;

[System.Serializable]
public class MultiDictionary<TKey, TValue> : IEnumerable {
	
	private Dictionary<TKey, List<TValue>> _data;

	public MultiDictionary(){
		_data = new Dictionary<TKey, List<TValue>>();
	}
	
	public void Add(TKey key, TValue value){
		if(_data.ContainsKey(key)){
			_data[key].Add(value);
	    } else {
			_data.Add(key, new List<TValue>() { value });
	    }
	}
	
  	public bool HasKey(TKey key){
    	return _data.ContainsKey(key);
  	}

  	public TValue[] GetValues(TKey key){
    	if(_data.ContainsKey(key)){
      		return _data[key].ToArray();
    	}

    	return null;
  	}

  	public void Replace(TKey key, TValue[] values){
    	if(_data.ContainsKey(key)){
      		_data[key] = new List<TValue>(values);
    	}
  	}

  	public void Remove(TKey key){
    	try {
      		_data.Remove(key);
    	} catch {
      		// There was no matching key, still correct execution
    	}
  	}

  	IEnumerator IEnumerable.GetEnumerator(){
    	return _data.GetEnumerator();
  	}
}