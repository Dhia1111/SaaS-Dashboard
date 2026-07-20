import {
  CircularChart3DComponent,
  CircularChart3DSeriesCollectionDirective,
  CircularChart3DSeriesDirective,
  PieSeries3D,
  Inject,
  CircularChartDataLabel3D,
  CircularChartLegend3D,
  CircularChartTooltip3D
} from "@syncfusion/ej2-react-charts";

const CHART_PALETTE = ["#2563eb", "#10b981", "#f59e0b"];

export default function Platform3DPieChart( {data ,id}) {
  
console.log("data",data);
  const chartData = data.map(item => {
    const detectedItemName = Object.keys(item).find(key => key !== "count");
    return { ...item, itemName: detectedItemName || "Unknown Item" };
  });

  return (
    <div className="py-8">
 <span className="text-[19px] font-bold text-indigo-600 bg-indigo-50  rounded-lg justify-center aline-center item-center" >{"Total "+data.length}</span>

      {data.length>0?<CircularChart3DComponent
        id={id}
        background="transparent"
        palettes={CHART_PALETTE}
        tilt={[-45]} // Sets the 3D viewing tilt angle
        legendSettings={{
          visible: true,
          position: "Bottom",
          alignment: "Center",
          textStyle: { fontFamily: "ui-sans-serif, system-ui, sans-serif", size: "11px", fontWeight: "600", color: "#64748b" }
        }}
        tooltip={{
          enable: true,
          format: "${x} : ${y}",
          fill: "#0f172a",
          border: { color: "#1e293b", width: 1 },
          textStyle: { fontFamily: "ui-sans-serif, system-ui, sans-serif", size: "11px", color: "#ffffff" }
        }}
      >
        <Inject services={[PieSeries3D, CircularChartDataLabel3D, CircularChartLegend3D, CircularChartTooltip3D]} />
        <CircularChart3DSeriesCollectionDirective>
          <CircularChart3DSeriesDirective
            dataSource={chartData}
            xName="platform"
            yName="count"
            radius="80%"
            dataLabel={{
              visible: true,
              position: "Inside",
              name: "count",
              font: { fontFamily: "ui-sans-serif, system-ui, sans-serif", fontWeight: "bold", color: "#ffffff", size: "11px" }
            }}
          />
        </CircularChart3DSeriesCollectionDirective>

      </CircularChart3DComponent>:<p className="text-sm text-slate-500">No Data to anlize</p>}



    </div>
  );
}