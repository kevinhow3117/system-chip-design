`timescale 1ns / 1ps
//////////////////////////////////////////////////////////////////////////////////
// Company: 
// Engineer: 
// 
// Create Date:    17:49:47 04/16/2022 
// Design Name: 
// Module Name:    breathing 
// Project Name: 
// Target Devices: 
// Tool versions: 
// Description: 
//
// Dependencies: 
//
// Revision: 
// Revision 0.01 - File Created
// Additional Comments: 
//
//////////////////////////////////////////////////////////////////////////////////
module breathing(clk,rst,sw,led);
	input clk,rst;
	input [3:0]sw;
	output reg led;
	reg [3:0]count;
	
	always@(posedge clk or posedge rst)
	begin
		if(rst)
		begin
			count<=4'd0;
			led<=1'b0;
		end
		else
		begin
			count<=count+1;
			if(sw>count)
			begin
				led<=1'b1;
				
			end
			else
			begin
				led<=1'b0;
			end
		end
	end
endmodule
