`timescale 1ns / 1ps

////////////////////////////////////////////////////////////////////////////////
// Company: 
// Engineer:
//
// Create Date:   17:57:44 04/16/2022
// Design Name:   breathing
// Module Name:   C:/Users/USER/Desktop/ISE FPGA/pin/breathing_LED/qq.v
// Project Name:  breathing_LED
// Target Device:  
// Tool versions:  
// Description: 
//
// Verilog Test Fixture created by ISE for module: breathing
//
// Dependencies:
// 
// Revision:
// Revision 0.01 - File Created
// Additional Comments:
// 
////////////////////////////////////////////////////////////////////////////////

module qq;

	// Inputs
	reg clk;
	reg rst;
	reg [3:0] sw;

	// Outputs
	wire led;

	// Instantiate the Unit Under Test (UUT)
	breathing uut (
		.clk(clk), 
		.rst(rst), 
		.sw(sw), 
		.led(led)
	);
	
	initial begin
	clk=0;
	forever #5 clk=~clk;
	end

	initial begin
		rst = 1;
		sw=4'b0110;
		#15;
      rst = 0;
		sw=4'b0110;
		#500
		sw=4'b0100;
		#500
		sw=4'b0111;
	end
      
endmodule

